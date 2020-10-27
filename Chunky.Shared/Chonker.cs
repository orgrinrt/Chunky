using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Chunky.Shared
{
    public class Chonker
    {
        private ColorRgba[,] _map;
        private int _chunkWidth;
        private int _chunkHeight;
        private short _chunkCountX;
        private short _chunkCountY;
        private bool _needsXFilling = true;
        private bool _needsYFilling = true;

        public Chonker(ColorRgba[,] map, int chunkWidth, int chunkHeight)
        {
            _map = map;
            _chunkWidth = chunkWidth;
            _chunkHeight = chunkHeight;

            SolveChunkDimensionalCount();
        }

        public Chonker(ColorRgba[,] map, short chunkCountX, short chunkCountY)
        {
            _map = map;
            _chunkCountX = chunkCountX;
            _chunkCountY = chunkCountY;

            SolveChunkSize();
        }
        
        /*
        public Chonker(short totalChunkCount)
        {
            SolveAll();
        }
        */
        
        public ChunkData[,] GenerateChunks()
        {
            ChunkData[,] result = new ChunkData[_chunkCountX, _chunkCountY];
            
            short originX, originY;

            for (int x = 0; x < _chunkCountX; x++)
            {
                for (int y = 0; y < _chunkCountY; y++)
                {
                    originX = (short) Math.Round((double) (x * _chunkWidth));
                    originY = (short) Math.Round((double) (y * _chunkHeight));

                    Bitmap bitmap = new Bitmap(_chunkWidth, _chunkHeight, PixelFormat.Format32bppRgb);
                    Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                    
                    System.Drawing.Imaging.BitmapData data = bitmap.LockBits(
                        rect, 
                        System.Drawing.Imaging.ImageLockMode.ReadWrite,
                        PixelFormat.Format32bppRgb);
                    
                    IntPtr ptr = data.Scan0;

                    int bytes = data.Stride * data.Height;
                    byte[] rgbValues = new byte[bytes];
                    int index = 0;
                    
                    System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                    for (int cy = originY; cy < originY + _chunkHeight; cy++)
                    {
                        for (int cx = originX; cx < originX + _chunkWidth; cx++)
                        {
                            //Console.WriteLine("X: " + ((cy - originY) + 1) + ", Y: " + (cx - originX));
                            for (int i = 0; i < 4; i++)
                            {
                                if (index > bytes - 1 ||
                                    cx > _map.GetLength(0) - 1 ||
                                    cy > _map.GetLength(1) - 1) break;

                                switch (i)
                                {
                                    case 0:
                                        rgbValues[index] = _map[cx, cy].B;
                                        break;
                                    case 1:
                                        rgbValues[index] = _map[cx, cy].G;
                                        break;
                                    case 2:
                                    rgbValues[index] = _map[cx, cy].R;
                                        break;
                                    case 3:
                                        rgbValues[index] = _map[cx, cy].A;
                                        break;
                                }
                                
                                index++;
                            }
                        }
                    }
                    
                    System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
                    
                    bitmap.UnlockBits(data);

                    result[x, y] = new ChunkData(bitmap, x, y);
                }
            }

            return result;
        }

        private void SolveChunkSize()
        {
            _chunkWidth = _map.GetLength(0) / _chunkCountX;
            _chunkHeight = _map.GetLength(1) / _chunkCountY;
        }

        private void SolveChunkDimensionalCount()
        {
            short width = (short)_map.GetLength(0);
            short height = (short) _map.GetLength(1);
            bool xExact = false;
            bool yExact = false;

            if (width % _chunkWidth == 0)
            {
            Console.WriteLine("NO REMAINDER FOR X");
                _chunkCountX = (short)(width / _chunkWidth);
                xExact = true;
            }
            else
            {
                _chunkCountX = (short)Math.Ceiling((double) (width / _chunkWidth));
                _chunkCountX++;
            }
            
            if (height % _chunkHeight == 0)
            {
                Console.WriteLine("NO REMAINDER FOR Y");
                _chunkCountY = (short)(height / _chunkHeight);
                yExact = true;
            }
            else
            {
                _chunkCountY = (short)Math.Ceiling((double) (height / _chunkHeight));
                _chunkCountY++;
            }

            // we can safely add extra buffer since we use original dimensions for reconstruction
            
            Console.WriteLine("SOLVED CHUNKXCOUNT: " + _chunkCountX);
            Console.WriteLine("     Total width: " + _chunkCountX * _chunkWidth);
            Console.WriteLine("SOLVED CHUNKYOUNT: " + _chunkCountY);
            Console.WriteLine("     Total height: " + _chunkCountY * _chunkHeight);

            if (xExact && yExact) return;
        }
    }
}