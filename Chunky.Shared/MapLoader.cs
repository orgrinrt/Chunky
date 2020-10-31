using System;
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
        private ColorRgba32bit[,] _map; // TODO: Probably not worth it to wrap it to a 2d map and unwrap on each operation... So maybe rewrite
        
        public Bitmap OriginalBitmap => _originalBitmap;
        public ColorRgba32bit[,] Map => _map;

        public MapLoader(string pathToMap)
        {
            _pathToMap = pathToMap;
            Load();
        }
        
        private ColorRgba32bit[,] Load()
        {
            if (String.IsNullOrEmpty(_pathToMap)) throw new Exception("Attempted to load a map without providing a path for the MapLoader");

            _originalBitmap = new Bitmap(_pathToMap);
            _map = new ColorRgba32bit[_originalBitmap.Width, _originalBitmap.Height];
            
            BitmapData data = _originalBitmap.LockBits(
                new Rectangle(0, 0, _originalBitmap.Width, _originalBitmap.Height), 
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
                    
            IntPtr ptr = data.Scan0;
            int bytes = data.Stride * data.Height;
            byte[] rgbValues = new byte[bytes];
            int byteIdx = 0;
                    
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            
            for (int y = 0; y < _originalBitmap.Height; y++)
            {
                for (int x = 0; x < _originalBitmap.Width; x++)
                {
                    _map[x, y] = new ColorRgba32bit(
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
        public ColorRgba32bit[,] Load(string pathToMap)
        {
            _pathToMap = pathToMap;
            return Load();
        }
    }
}