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
        
        public Chonker(ColorRgba[,] map, short chunkCountX, int chunkHeight)
        {
            _map = map;
            _chunkCountX = chunkCountX;
            _chunkHeight = chunkHeight;

            SolveChunkDimensionalCount();
            SolveChunkSize();
        }
        
        public Chonker(ColorRgba[,] map, int chunkHeight, short chunkCountY)
        {
            _map = map;
            _chunkCountY = chunkCountY;
            _chunkHeight = chunkHeight;

            SolveChunkDimensionalCount();
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

            // TODO: We should definitely chonk-a-bonk each chunk in a separate task for multithreading experience
            
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
            if (_chunkCountX != default) _chunkWidth = _map.GetLength(0) / _chunkCountX;
            if (_chunkCountY != default) _chunkHeight = _map.GetLength(1) / _chunkCountY;
        }

        private void SolveChunkDimensionalCount()
        {
            short width = (short)_map.GetLength(0);
            short height = (short) _map.GetLength(1);

            if (_chunkWidth != default)
            {
                if (width % _chunkWidth == 0)
                {
                    Console.WriteLine("NO REMAINDER FOR X");
                    _chunkCountX = (short)(width / _chunkWidth);
                }
                else
                {
                    _chunkCountX = (short)Math.Ceiling((double) (width / _chunkWidth));
                    _chunkCountX++; // add an extra buffer for safety
                }
            }
            if (_chunkHeight != default)
            {
                if (height % _chunkHeight == 0)
                {
                    Console.WriteLine("NO REMAINDER FOR Y");
                    if (_chunkHeight != default) _chunkCountY = (short)(height / _chunkHeight);
                }
                else
                {
                    _chunkCountY = (short)Math.Ceiling((double) (height / _chunkHeight));
                    _chunkCountY++; // add an extra buffer for safety
                }
            }
        }
    }
}