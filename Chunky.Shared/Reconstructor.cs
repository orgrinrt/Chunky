using System;
using System.Collections.Generic;
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

        /// <summary>
        /// A little bit heftier version of Reconstruct, that also generates some exports to compare the resulting reconstruction
        /// against the original pixel by pixel. Should allow for easier dev time but also give sanity points to anyone
        /// anxious about using the program (i.e they can set the flag to also generate and export the comparisons and see
        /// how close the chunks are to the original).
        /// </summary>
        /// <param name="targetPathDir">Optional save path dir. NOTE: has to point to a dir, since this will generate multiple bitmaps.</param>
        /// <returns>The reconstruction (element 0) as well as comparison/debugging bitmaps.</returns>
        public Bitmap[] ReconstructAndCompare(Bitmap original, string targetPathDir = null)
        {
            List<Bitmap> result = new List<Bitmap>();
            
            Bitmap reconstruction = Reconstruct(targetPathDir);
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
        public Bitmap Reconstruct(string targetPathDir = null)
        {
            // TODO: I did poopy. Please take the time to revisit and rewrite this whole mess. (it does work as-is so, uh, "soon")
        
            // we assume that each chonky-chonk is uniform in size
            // wouldn't be hard to make it go through each individually but that's just overhead we don't need since it chonks gud
            /*Bitmap result = new Bitmap(
                _data.GetLength(0) * _data[0,0].Bitmap.Width,
                _data.GetLength(1) * _data[0,0].Bitmap.Height);*/
            Bitmap result = new Bitmap(_originalWidth, _originalHeight);

            Rectangle rect = Rectangle.Empty;
            BitmapData bitmapData = null;
                    
            IntPtr memStartPointer = IntPtr.Zero;
            int bytes = 0;
            byte[] rgbValues = new byte[0];
            int resultIndex = 0;
            int count = 0;

            Bitmap chunkBitmap = null;
            Rectangle chunkRect = Rectangle.Empty;
            BitmapData chunkBitmapData = null;
            IntPtr chunkMemStartPointer = IntPtr.Zero;
            int chunkBytes = 0;
            byte[] chunkRgbValues = new byte[0];
            int chunkIndex = 0;
            short diffX = 0;
            short diffY = 0;
            short chunkWidth = 0;
            short chunkHeight = 0;

            for (int x = 0; x < _data.GetLength(0); x++)
            {
                for (int y = 0; y < _data.GetLength(1); y++)
                {
                    chunkBitmap = _data[x, y].Bitmap;

                    if (chunkWidth == 0) chunkWidth = (short)chunkBitmap.Width;
                    if (chunkHeight == 0) chunkHeight = (short)chunkBitmap.Height;

                    if (x == _data.GetLength(0) - 1)
                        diffX = (short) ((short) ((x * chunkWidth) + chunkWidth) - _originalWidth);
                    else diffX = 0;
                    if (y == _data.GetLength(1) - 1)
                        diffY = (short) ((short) ((y * chunkHeight) + chunkHeight) - _originalHeight);
                    else diffY = 0;

                    // the below works but we'd rather juts skip having completely empty chunks in the buffer in the first place
                    //if (chunkBitmap.Width - diffX <= 0 || chunkBitmap.Height - diffY <= 0) continue;
                    
                    chunkRect = new Rectangle(0, 0, chunkWidth, chunkHeight);
                    
                    
                    rect = new Rectangle(x * chunkWidth, y * chunkHeight, 
                        chunkBitmap.Width - diffX, 
                        chunkBitmap.Height - diffY);

                    Console.WriteLine("-------");
                    Console.WriteLine("X: " + x  + " / " + _data.GetLength(0) + ", Y: " + y + " / " + _data.GetLength(1));
                    Console.WriteLine("X DIFF: " + diffX);
                    Console.WriteLine("Y DIFF: " + diffY);
                    Console.WriteLine("X BOB: " + chunkBitmap.Width);
                    Console.WriteLine("Y BOB: " + chunkBitmap.Height);
                    Console.WriteLine("THEORY X LENGTH " + ((x * chunkWidth + chunkWidth) - diffX));
                    Console.WriteLine("THEORY Y LENGTH " + ((y * chunkHeight + chunkHeight) - diffY));
                    Console.WriteLine("ACTUAL X LENGTH " + ((x * chunkWidth + chunkBitmap.Width)));
                    Console.WriteLine("ACTUAL Y LENGTH " + ((y * chunkHeight + chunkBitmap.Height)));
                    Console.WriteLine("RESULT X LENGTH: " + result.Width);
                    Console.WriteLine("RESULT Y LENGTH: " + result.Height);
                    
                    bitmapData = result.LockBits(
                        rect, 
                        ImageLockMode.ReadWrite,
                        PixelFormat.Format32bppRgb);
                    chunkBitmapData = chunkBitmap.LockBits(
                        chunkRect, 
                        ImageLockMode.ReadOnly,
                        PixelFormat.Format32bppRgb);
                    
                    /*If (count == 0)*/ memStartPointer = bitmapData.Scan0;
                    /*if (count == 0)*/ bytes = bitmapData.Stride * bitmapData.Height;
                    /*if (count == 0)*/ rgbValues = new byte[bytes];
                    
                    /*if (count == 0)*/ chunkMemStartPointer = chunkBitmapData.Scan0;
                    /*if (count == 0)*/ chunkBytes = chunkBitmapData.Stride * chunkBitmapData.Height;
                    /*if (count == 0)*/ chunkRgbValues = new byte[chunkBytes];
                
                    System.Runtime.InteropServices.Marshal.Copy(memStartPointer, rgbValues, 0, bytes);
                    System.Runtime.InteropServices.Marshal.Copy(chunkMemStartPointer, chunkRgbValues, 0, chunkBytes);
                
                    // TODO: just use a for loop to the length of the `bytes` var, man...
                    for (int cy = 0; cy < chunkHeight; cy++)
                    {
                        for (int cx = 0; cx < chunkWidth; cx++)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                if (resultIndex > bytes - 1 || chunkIndex > chunkBytes - 1) break;
                                rgbValues[resultIndex] = chunkRgbValues[chunkIndex];
                                resultIndex++;
                                chunkIndex++;
                            }
                        }
                    }
                    
                    System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, memStartPointer, bytes);
                    chunkBitmap.UnlockBits(chunkBitmapData);
                    result.UnlockBits(bitmapData);
                    chunkIndex = 0;
                    resultIndex = 0;
                    count++;
                }
            }

            if (targetPathDir != null) 
                result.Save(Path.Combine(targetPathDir, _name + "-reconstruction" + Utils.FormatExtension(ImageFormat.Png)));
            _reconstruction = result;
            return result;
        }

        public Bitmap CompareVariance(Bitmap original, string targetPathDir = null)
        {
            Bitmap result = new Bitmap(original.Width, original.Height);
            Bitmap reconstructino = _reconstruction;

            Console.WriteLine("ORIGINAL: " + original.Width + ", " + original.Height);
            Console.WriteLine("RECONSTR: " + reconstructino.Width + ", " + reconstructino.Height);
            
            Rectangle rect = new Rectangle(0, 0, original.Width, original.Height);
            
            BitmapData originalData = original.LockBits(
                rect, 
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppRgb);
            BitmapData reconstructionData = reconstructino.LockBits(
                rect, 
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppRgb);
            BitmapData resultData = result.LockBits(
                rect, 
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppRgb);
                    
            IntPtr originalPointer = originalData.Scan0;
            IntPtr reconstructionPointer = reconstructionData.Scan0;
            IntPtr resultPointer = resultData.Scan0;
            
            int bytes = originalData.Stride * originalData.Height;
            byte[] originalRgbValues = new byte[bytes];
            byte[] reconstructionRgbValues = new byte[bytes];
            byte[] resultRgbValues = new byte[bytes];
                
            System.Runtime.InteropServices.Marshal.Copy(originalPointer, originalRgbValues, 0, bytes);
            System.Runtime.InteropServices.Marshal.Copy(reconstructionPointer, reconstructionRgbValues, 0, bytes);
            System.Runtime.InteropServices.Marshal.Copy(resultPointer, resultRgbValues, 0, bytes);

            byte currR = 0;
            byte diffR = 0;
            byte diffG = 0;
            byte diffB = 0;
            byte diffA = 0;
            byte byteIdx = 0;
            
            for (int i = 0; i < bytes; i++)
            {
                switch (byteIdx)
                {
                    // jesus these colors arent in ARGB or RGBA order, they are apparently BGRA.......................
                    case 2:
                        diffR = (byte) Math.Abs(originalRgbValues[i] - reconstructionRgbValues[i]);
                        currR = reconstructionRgbValues[i];
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
                            /*
                        resultRgbValues[i - 3] = 255;
                        resultRgbValues[i - 2] = 0;
                        resultRgbValues[i - 1] = 0;
                        resultRgbValues[i] = originalRgbValues[i];
                        */
                        
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
                        }
                        byteIdx = 0;
                        continue;
                    }
                }
                byteIdx++;
            }
            
            System.Runtime.InteropServices.Marshal.Copy(resultRgbValues, 0, resultPointer, bytes);
            original.UnlockBits(originalData);
            reconstructino.UnlockBits(reconstructionData);
            result.UnlockBits(resultData);
            
            Console.WriteLine(reconstructino.GetPixel(500, 1200));
            Console.WriteLine(original.GetPixel(500, 1200));
            Console.WriteLine(result.GetPixel(500, 1200));
            Console.WriteLine(reconstructino.GetPixel(800, 800));
            Console.WriteLine(original.GetPixel(800, 800));
            Console.WriteLine(result.GetPixel(800, 800));
            Console.WriteLine(reconstructino.GetPixel(1200, 500));
            Console.WriteLine(original.GetPixel(1200, 500));
            Console.WriteLine(result.GetPixel(1200, 500));
            
            if (targetPathDir != null) 
                result.Save(Path.Combine(targetPathDir, _name + "-reconstruction-variance" + Utils.FormatExtension(ImageFormat.Png)));
            return result;
        }
    }
}