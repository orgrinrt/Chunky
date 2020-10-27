using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Chunky.Shared
{
    public class Chonker
    {
        private byte[,] _map;
        private int _chunkWidth;
        private int _chunkHeight;
        private short _chunkCountX;
        private short _chunkCountY;
        private bool _needsXFilling = true;
        private bool _needsYFilling = true;

        public Chonker(byte[,] map, int chunkWidth, int chunkHeight)
        {
            _map = map;
            _chunkWidth = chunkWidth;
            _chunkHeight = chunkHeight;

            SolveChunkDimensionalCount();
        }

        public Chonker(byte[,] map, short chunkCountX, short chunkCountY)
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
        
        public ChunkData[] GenerateChunks()
        {
            List<ChunkData> result = new List<ChunkData>();
            
            short originX, originY;

            for (int x = 0; x < _chunkCountX; x++)
            {
                for (int y = 0; y < _chunkCountY; y++)
                {
                    originX = (short) Math.Round((double) (x * _chunkWidth));
                    originY = (short) Math.Round((double) (y * _chunkHeight));

                    Console.WriteLine("originX: " + originX);
                    Console.WriteLine("originY: " + originY);
                    Console.WriteLine("chunkSizeX: " + _chunkWidth);
                    Console.WriteLine("chunkSizeY: " + _chunkHeight);
                    Console.WriteLine("-------");

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
                                if (index + i > bytes - 1) break;
                                rgbValues[index] = _map[cx, cy];
                                index++;
                            }
                        }
                    }
                    
                    System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
                    
                    bitmap.UnlockBits(data);

                    result.Add(new ChunkData(bitmap, x, y));
                }
            }

            return result.ToArray();
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
                _chunkCountX = (short)(width / _chunkWidth);
                xExact = true;
            }
            else
            {
                _chunkCountX = (short)Math.Ceiling((double) (width / _chunkWidth));
            }
            
            if (height % _chunkHeight == 0)
            {
                _chunkCountY = (short)(height / _chunkHeight);
                yExact = true;
            }
            else
            {
                _chunkCountY = (short)Math.Ceiling((double) (height / _chunkHeight));
            }

            if (xExact && yExact) return;
        }
    }
}