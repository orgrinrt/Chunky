﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Chunky.Shared
{
    public class MapLoader
    {
        private string _pathToMap;
        private Bitmap _originalBitmap;
        private ColorRgba[,] _map;
        
        public Bitmap OriginalBitmap => _originalBitmap;
        public ColorRgba[,] Map => _map;

        public MapLoader(string pathToMap)
        {
            _pathToMap = pathToMap;
            Load();
        }
        
        private ColorRgba[,] Load()
        {
            if (String.IsNullOrEmpty(_pathToMap)) throw new Exception("Attempted to load a map without providing a path to the MapLoader");

            _originalBitmap = new Bitmap(_pathToMap);
            _map = new ColorRgba[_originalBitmap.Width, _originalBitmap.Height];
            
            BitmapData data = _originalBitmap.LockBits(
                new Rectangle(0, 0, _originalBitmap.Width, _originalBitmap.Height), 
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppRgb);
                    
            IntPtr ptr = data.Scan0;
            int bytes = data.Stride * data.Height;
            byte[] rgbValues = new byte[bytes];
            int byteIdx = 0;
                    
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            
            for (int y = 0; y < _originalBitmap.Height; y++)
            {
                for (int x = 0; x < _originalBitmap.Width; x++)
                {
                    _map[x, y] = new ColorRgba(
                        rgbValues[byteIdx+2],
                        rgbValues[byteIdx+1],
                        rgbValues[byteIdx],
                        rgbValues[byteIdx+3]
                    );
                    byteIdx += 4;
                }
            }
            
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            _originalBitmap.UnlockBits(data);
            return _map;
        }
        
        /// <summary>
        /// A public Load method for re-using same instance for multiple jobs
        /// </summary>
        /// <param name="pathToMap">A path to the source image</param>
        public ColorRgba[,] Load(string pathToMap)
        {
            _pathToMap = pathToMap;
            return Load();
        }
    }
}