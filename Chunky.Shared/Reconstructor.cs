using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Chunky.Shared
{
    public class Reconstructor
    {
        private string _name;
        private ImageFormat _format;
        private ChunkData[,] _data;
        private Bitmap _reconstruction;

        private short _originalWidth;
        private short _originalHeight;

        private byte _lighterThreshold = 230;
        private float _diffMultiplier = 7f;

        public ChunkData[,] Data => _data;

        public Reconstructor(ChunkData[,] data, string name, short originalWidth, short originalHeight, ImageFormat format = null)
        {
            _originalWidth = originalWidth;
            _originalHeight = originalHeight;
            _name = name;
            _data = data;
            if (format == null) _format = ImageFormat.Png;
        }

        public Reconstructor(string resultDir, string name, short originalWidth, short originalHeight, ImageFormat format = null)
        {
            _originalWidth = originalWidth;
            _originalHeight = originalHeight;
            _name = name;
            if (format == null) _format = ImageFormat.Png;

            List<ChunkData> result = new List<ChunkData>();
            
            if (!Directory.Exists(resultDir)) throw new Exception("Attempted to reconstruct from a result dir that doesn't exist");

            string[] files = Directory.GetFiles(resultDir);
            short maxX = short.MinValue;
            short maxY = short.MinValue;

            foreach (string filePath in files)
            {
                string[] split = filePath.Split('.');
                string[] split2 = split[^2].Split('-');
                
                if (!short.TryParse(split2[^1], out short y))
                {
                    continue;
                }

                if (!short.TryParse(split2[^2], out short x))
                {
                    continue;
                }

                if (y > maxY) maxY = y;
                if (x > maxX) maxX = x;

                Bitmap bitmap = new Bitmap(filePath);/*
                Bitmap processed = bitmap.Clone(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    PixelFormat.Format32bppArgb);*/
                
                result.Add(new ChunkData(bitmap, x, y));
            }

            _data = new ChunkData[maxX + 1, maxY + 1];

            for (int x = 0; x < _data.GetLength(0); x++)
            {
                for (int y = 0; y < _data.GetLength(1); y++)
                {
                    foreach (ChunkData chunk in result)
                    {
                        if (chunk.X == x && chunk.Y == y) _data[x, y] = chunk;
                    }
                }
            }
        }

        /// <summary>
        /// A little bit heftier version of Reconstruct, that also generates some exports to compare the resulting reconstruction
        /// against the original pixel by pixel. Should allow for easier dev time but also give sanity points to anyone
        /// anxious about using the program (i.e they can set the flag to also generate and export the comparisons and see
        /// how close the chunks are to the original).
        /// </summary>
        /// <param name="targetPathDir">Optional save path dir. NOTE: has to point to a dir, since this will generate multiple bitmaps.</param>
        /// <returns>The reconstruction (element 0) as well as comparison/debugging bitmaps.</returns>
        public Bitmap[] ReconstructAndCompare(Bitmap original, string targetPathDir = null, bool compatibilityMode = false)
        {
            List<Bitmap> result = new List<Bitmap>();
            
            Bitmap reconstruction = Reconstruct(targetPathDir, compatibilityMode);
            result.Add(reconstruction);

            Bitmap varianceColored = CompareVariance(original, targetPathDir);
            result.Add(varianceColored);

            return result.ToArray();
        }

        /// <summary>
        /// Reconstructs the resulting chonkyarray back to a wholy-whole pic-pic
        /// Takes in an optional path that also saves the resulting reconstruction in same pass
        /// </summary>
        /// <param name="targetPath">Optional save path dir. NOTE: has to point to a dir, since this will generate multiple bitmaps.</param>
        /// <returns>The reconstruction</returns>
        public Bitmap Reconstruct(string targetPathDir = null, bool compatibilityMode = false)
        {
            Bitmap result = new Bitmap(_originalWidth, _originalHeight);
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Rectangle resultRect = Rectangle.Empty;
            Rectangle chunkRect = Rectangle.Empty;
            
            short chunkWidth = 0, chunkHeight = 0;
            byte[] rgbValues = new byte[0];
            byte[] chunkRgbValues = new byte[0];

            for (int x = 0; x < _data.GetLength(0); x++)
            {
                for (int y = 0; y < _data.GetLength(1); y++)
                {
                    int x1 = x;
                    int y1 = y;

                    Bitmap chunkBitmap = _data[x1, y1].Bitmap;
                    short diffX, diffY;

                    if (chunkBitmap == null) throw new Exception("Attempting to reconstruct from an out-of-bounds map (no chunk exists but map has index)");                    
                    
                    if (chunkWidth == 0) chunkWidth = (short)chunkBitmap.Width;
                    if (chunkHeight == 0) chunkHeight = (short)chunkBitmap.Height;

                    if (x1 == _data.GetLength(0) - 1) diffX = (short) ((short) ((x1 * chunkWidth) + chunkWidth) - _originalWidth);
                    else diffX = 0;
                    if (y1 == _data.GetLength(1) - 1) diffY = (short) ((short) ((y1 * chunkHeight) + chunkHeight) - _originalHeight);
                    else diffY = 0;

                    if (chunkBitmap.Width - diffX <= 0 || chunkBitmap.Height - diffY <= 0) throw new Exception("Found an empty chunk! Probably want to see what causes this.");

                    chunkRect.X = 0; 
                    chunkRect.Y = 0;
                    chunkRect.Width = chunkWidth; 
                    chunkRect.Height = chunkHeight;
                    
                    resultRect.X = x1 * chunkWidth;
                    resultRect.Y = y1 * chunkHeight;
                    resultRect.Width = Math.Min((chunkBitmap.Width - Math.Max((int) diffX, 0)), _originalWidth - x1 * chunkBitmap.Width);
                    resultRect.Height = Math.Min((chunkBitmap.Height - Math.Max((int) diffY, 0)), _originalHeight - y1 * chunkBitmap.Height);

                    BitmapData bitmapData = result.LockBits(
                        resultRect, 
                        ImageLockMode.ReadWrite,
                        PixelFormat.Format32bppPArgb);
                    BitmapData chunkBitmapData = chunkBitmap.LockBits(
                        chunkRect, 
                        ImageLockMode.ReadOnly,
                        PixelFormat.Format32bppPArgb);
                
                    IntPtr resultPtr = bitmapData.Scan0;
                    IntPtr chunkPtr = chunkBitmapData.Scan0;
                    int bytes = bitmapData.Stride * bitmapData.Height;
                    if (rgbValues.Length != bytes) rgbValues = new byte[bytes];
                    if (chunkRgbValues.Length != bytes) chunkRgbValues = new byte[bytes];

                    System.Runtime.InteropServices.Marshal.Copy(resultPtr, rgbValues, 0, bytes);
                    System.Runtime.InteropServices.Marshal.Copy(chunkPtr, chunkRgbValues, 0, bytes);

                    for (int i = 0; i < bytes; i++)
                    {
                        rgbValues[i] = chunkRgbValues[i];
                    }
                
                    System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, resultPtr, bytes);
                
                    chunkBitmap.UnlockBits(chunkBitmapData);
                    result.UnlockBits(bitmapData);
                }
            }

            if (targetPathDir != null) 
                result.Save(Path.Combine(targetPathDir, _name + "-reconstruction" + Utils.FormatExtension(ImageFormat.Png)));
            _reconstruction = result;

            Console.WriteLine(sw.ElapsedMilliseconds);
            sw.Stop();
            return result;
        }
        
        public Bitmap CompareVariance(Bitmap original, string targetPathDir = null)
        {
            Bitmap result = new Bitmap(original.Width, original.Height);
            Bitmap reconstruction = _reconstruction;

            /*
            Console.WriteLine("ORIGINAL: " + original.Width + ", " + original.Height);
            Console.WriteLine("RECONSTR: " + reconstructino.Width + ", " + reconstructino.Height);
            */
            
            Rectangle rect = new Rectangle(0, 0, original.Width, original.Height);
            
            BitmapData originalData = original.LockBits(
                rect, 
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppPArgb);
            BitmapData reconstructionData = reconstruction.LockBits(
                rect, 
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppPArgb);
            BitmapData resultData = result.LockBits(
                rect, 
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppPArgb);
                    
            IntPtr originalPtr = originalData.Scan0;
            IntPtr reconstructionPtr = reconstructionData.Scan0;
            IntPtr resultPtr = resultData.Scan0;
            
            int bytes = originalData.Stride * originalData.Height;
            byte[] originalRgbValues = new byte[bytes];
            byte[] reconstructionRgbValues = new byte[bytes];
            byte[] resultRgbValues = new byte[bytes];
                
            System.Runtime.InteropServices.Marshal.Copy(originalPtr, originalRgbValues, 0, bytes);
            System.Runtime.InteropServices.Marshal.Copy(reconstructionPtr, reconstructionRgbValues, 0, bytes);
            System.Runtime.InteropServices.Marshal.Copy(resultPtr, resultRgbValues, 0, bytes);

            byte diffR = 0, diffG = 0, diffB = 0, diffA = 0, byteIdx = 0/*, currR = 0*/;
            
            for (int i = 0; i < bytes; i++)
            {
                switch (byteIdx)
                {
                    // note to self: ordered BGRA
                    case 2:
                        diffR = (byte) Math.Abs(originalRgbValues[i] - reconstructionRgbValues[i]);
                        //currR = reconstructionRgbValues[i];
                        break;
                    case 1:
                        diffG = (byte) Math.Abs(originalRgbValues[i] - reconstructionRgbValues[i]);
                        break;
                    case 0:
                        diffB = (byte) Math.Abs(originalRgbValues[i] - reconstructionRgbValues[i]);
                        break;
                    case 3:
                    {
                        diffA = (byte) Math.Abs(originalRgbValues[i] - reconstructionRgbValues[i]);
                        byte avgDiff = (byte) ((diffR + diffG + diffB + diffA) / 4);
                        //byte avgDiff = (byte) (diffR);

                        if (avgDiff > 0)
                        {
                            resultRgbValues[i - 3] = 0;
                            resultRgbValues[i - 2] = 0;
                            resultRgbValues[i - 1] = 255;
                            resultRgbValues[i] = originalRgbValues[i];

                        /*
                            if (currR > _lighterThreshold)
                            {
                                resultRgbValues[i - 3] = (byte)Math.Clamp(originalRgbValues[i - 3] - (avgDiff * _diffMultiplier), 0, 255);
                                resultRgbValues[i - 2] = (byte)Math.Clamp(originalRgbValues[i - 2] - (avgDiff * _diffMultiplier), 0, 255);
                                resultRgbValues[i - 1] = (byte)Math.Clamp(originalRgbValues[i - 1] + (avgDiff * (_diffMultiplier / 2)), 0, 255);
                                //resultRgbValues[i-3] = originalRgbValues[i-3];
                                //resultRgbValues[i-2] = originalRgbValues[i-2];
                                //resultRgbValues[i-1] = originalRgbValues[i-1];
                                resultRgbValues[i] = originalRgbValues[i];
                            }
                            else
                            {
                                resultRgbValues[i - 3] = (byte)Math.Clamp(originalRgbValues[i - 3] - (avgDiff * (_diffMultiplier / 2)), 0, 255);
                                resultRgbValues[i - 2] = (byte)Math.Clamp(originalRgbValues[i - 2] - (avgDiff * (_diffMultiplier / 2)), 0, 255);
                                resultRgbValues[i - 1] = (byte)Math.Clamp(originalRgbValues[i - 1] + (avgDiff * _diffMultiplier), 0, 255);
                                //resultRgbValues[i-3] = originalRgbValues[i-3];
                                //resultRgbValues[i-2] = originalRgbValues[i-2];
                                //resultRgbValues[i-1] = originalRgbValues[i-1];
                                resultRgbValues[i] = originalRgbValues[i];
                            }
                        }
                        else
                        {
                            resultRgbValues[i-3] = originalRgbValues[i-3];
                            resultRgbValues[i-2] = originalRgbValues[i-2];
                            resultRgbValues[i-1] = originalRgbValues[i-1];
                            resultRgbValues[i] = originalRgbValues[i];
                        */
                        }
                        
                        
                        byteIdx = 0;
                        continue;
                    }
                }
                byteIdx++;
            }
            
            System.Runtime.InteropServices.Marshal.Copy(resultRgbValues, 0, resultPtr, bytes);
            original.UnlockBits(originalData);
            reconstruction.UnlockBits(reconstructionData);
            result.UnlockBits(resultData);
            
            /*
            Console.WriteLine(reconstructino.GetPixel(500, 1200));
            Console.WriteLine(original.GetPixel(500, 1200));
            Console.WriteLine(result.GetPixel(500, 1200));
            Console.WriteLine(reconstructino.GetPixel(800, 800));
            Console.WriteLine(original.GetPixel(800, 800));
            Console.WriteLine(result.GetPixel(800, 800));
            Console.WriteLine(reconstructino.GetPixel(1200, 500));
            Console.WriteLine(original.GetPixel(1200, 500));
            Console.WriteLine(result.GetPixel(1200, 500));
            */
            
            if (targetPathDir != null) 
                result.Save(Path.Combine(targetPathDir, _name + "-reconstruction-variance" + Utils.FormatExtension(ImageFormat.Png)));
            return result;
        }
    }
}