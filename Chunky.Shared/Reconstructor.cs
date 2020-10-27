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
        private ReconstructMode _mode = ReconstructMode.Normal;
        
        public ChunkData[,] Data => _data;

        public Reconstructor(ChunkData[,] data, string name, ImageFormat format = null)
        {
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
        public Bitmap[] ReconstructAndCompare(string targetPathDir = null)
        {
            List<Bitmap> result = new List<Bitmap>();
            Bitmap reconstruction = Reconstruct(targetPathDir);
            
            result.Add(reconstruction);

            _mode = ReconstructMode.ColorVariance;

            //Bitmap varianceColored = Reconstruct(targetPathDir);

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
            // we assume that each chonky-chonk is uniform in size
            // wouldn't be hard to make it go through each individually but that's just overhead we don't need since it chonks gud
            Bitmap result = new Bitmap(
                _data.GetLength(0) * _data[0,0].Bitmap.Width,
                _data.GetLength(1) * _data[0,0].Bitmap.Height);

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

            for (int x = 0; x < _data.GetLength(0); x++)
            {
                for (int y = 0; y < _data.GetLength(1); y++)
                {
                    chunkBitmap = _data[x,y].Bitmap;
                    
                    /*if (count == 0) */chunkRect = new Rectangle(0, 0, chunkBitmap.Width, chunkBitmap.Height);
                    
                    rect = new Rectangle(x * chunkBitmap.Width, y * chunkBitmap.Height, chunkBitmap.Width, chunkBitmap.Height);
                    
                    bitmapData = result.LockBits(
                        rect, 
                        ImageLockMode.ReadWrite,
                        PixelFormat.Format32bppRgb);
                    
                    /*if (count == 0) */memStartPointer = bitmapData.Scan0;
                    /*if (count == 0) */bytes = bitmapData.Stride * bitmapData.Height;
                    /*if (count == 0) */rgbValues = new byte[bytes];
                    
                    System.Runtime.InteropServices.Marshal.Copy(memStartPointer, rgbValues, 0, bytes);
                    
                    chunkBitmapData = chunkBitmap.LockBits(
                        chunkRect, 
                        ImageLockMode.ReadOnly,
                        PixelFormat.Format32bppRgb);
                    
                    /*if (count == 0) */chunkMemStartPointer = chunkBitmapData.Scan0;
                    /*if (count == 0) */chunkBytes = chunkBitmapData.Stride * chunkBitmapData.Height;
                    /*if (count == 0) */chunkRgbValues = new byte[chunkBytes];
                
                    System.Runtime.InteropServices.Marshal.Copy(chunkMemStartPointer, chunkRgbValues, 0, chunkBytes);
                
                    for (int cy = 0; cy < chunkBitmap.Height; cy++)
                    {
                        for (int cx = 0; cx < chunkBitmap.Width; cx++)
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
                    
                    chunkBitmap.UnlockBits(chunkBitmapData);
                    System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, memStartPointer, bytes);
                    result.UnlockBits(bitmapData);
                    chunkIndex = 0;
                    resultIndex = 0;
                    count++;
                }
            }

            if (targetPathDir != null)
            {
                string mode = _mode != ReconstructMode.Normal ? "-" + _mode.ToString().ToLower() : "";
                result.Save(
                Path.Combine(targetPathDir, _name + "-reconstruction" + mode + Utils.FormatExtension(ImageFormat.Png)));
            }
            return result;
        }
    }

    public enum ReconstructMode
    {
        Normal,
        ColorVariance
    }
}