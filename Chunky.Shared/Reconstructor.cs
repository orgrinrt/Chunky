using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Chunky.Shared
{
    public class Reconstructor
    {
        private ChunkData[,] _data;
        
        public ChunkData[,] Data => _data;

        public Reconstructor(ChunkData[,] data)
        {
            _data = data;
        }

        /// <summary>
        /// Reconstructs the resulting chonkyarray back to a wholy-whole pic-pic
        /// Takes in an optional path that also saves the resulting reconstruction in same pass
        /// </summary>
        /// <param name="targetPath">Optional save path</param>
        /// <returns>The reconstruction</returns>
        public Bitmap Reconstruct(string targetPath = null)
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

            if (targetPath != null) result.Save(targetPath, ImageFormat.Png);
            return result;
        }
    }
}