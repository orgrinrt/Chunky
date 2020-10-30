using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace Chunky.Shared
{
    public class Chonker
    {
        private readonly ColorRgba32bit[,] _map;
        private int _chunkWidth;
        private int _chunkHeight;
        private short _chunkCountX;
        private short _chunkCountY;

        private int _totalChunkCount;
        private ChunkData[,] _processedChunks;

        public Chonker(ColorRgba32bit[,] map, int chunkWidth, int chunkHeight)
        {
            _map = map;
            _chunkWidth = chunkWidth;
            _chunkHeight = chunkHeight;

            SolveChunkDimensionalCount();
        }
        public Chonker(ColorRgba32bit[,] map, short chunkCountX, short chunkCountY)
        {
            _map = map;
            _chunkCountX = chunkCountX;
            _chunkCountY = chunkCountY;

            SolveChunkSize();
        }
        
        public Chonker(ColorRgba32bit[,] map, short chunkCountX, int chunkHeight)
        {
            _map = map;
            _chunkCountX = chunkCountX;
            _chunkHeight = chunkHeight;

            SolveChunkDimensionalCount();
            SolveChunkSize();
        }
        
        public Chonker(ColorRgba32bit[,] map, int chunkHeight, short chunkCountY)
        {
            _map = map;
            _chunkCountY = chunkCountY;
            _chunkHeight = chunkHeight;

            SolveChunkDimensionalCount();
            SolveChunkSize();
        }
        
        public ChunkData[,] GenerateChunks()
        {/*
            Console.WriteLine("CHUNKWIDTH: " + _chunkWidth);
            Console.WriteLine("CHUNKHEIGHT: " + _chunkHeight);
            Console.WriteLine("CHUNK COUNTX " + _chunkCountX);
            Console.WriteLine("CHUNK COUNTY: " + _chunkCountY);*/
            _processedChunks = new ChunkData[_chunkCountX, _chunkCountY];
            _totalChunkCount = _chunkCountX * _chunkCountY;
            int _processedCount = 0;
            object countLock = new object();
            
            for (int x = 0; x < _chunkCountX; x++)
            {
                for (int y = 0; y < _chunkCountY; y++)
                {
                    // process each chunk in its own thread
                    // we know it'll be fine because each operation only adds an element to a unique index in a pre-allocated array
                    int x1 = x;
                    int y1 = y;
                    
                    Thread thread = new Thread(() =>
                    {
                        _processedChunks[x1, y1] = GenerateChunk(x1, y1);
                        lock (countLock) _processedCount++;
                    });
                    thread.Start();
                }
            }
            
            while (_processedCount < _totalChunkCount) { }

            return _processedChunks;
        }

        private ChunkData GenerateChunk(int x, int y)
        {
            short originX = (short) Math.Round((double) (x * _chunkWidth));
            short originY = (short) Math.Round((double) (y * _chunkHeight));

            Bitmap bitmap = new Bitmap(_chunkWidth, _chunkHeight, PixelFormat.Format32bppRgb);
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            
            BitmapData data = bitmap.LockBits(
                rect, 
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);
            
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
            return new ChunkData(bitmap, x, y);
        }

        private void SolveChunkSize()
        {
            Console.WriteLine("SOLVE CHUNK SIZE");
            if (_chunkCountX != default) _chunkWidth = _map.GetLength(0) / _chunkCountX;
            if (_chunkCountY != default) _chunkHeight = _map.GetLength(1) / _chunkCountY;
        }

        private void SolveChunkDimensionalCount()
        {
            Console.WriteLine("SOLVE CHUNK DIMENSIONAL COUNT");
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