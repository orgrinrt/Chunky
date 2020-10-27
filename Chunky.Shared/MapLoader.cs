using System;
using System.Drawing;

namespace Chunky.Shared
{
    public class MapLoader
    {
        private string _pathToMap;
        private byte[,] _map;

        public byte[,] Map
        {
            get => _map;
            set => _map = value;
        }

        public MapLoader(string pathToMap)
        {
            _pathToMap = pathToMap;

            Load();
        }

        public MapLoader()
        {
            
        }

        /// <summary>
        /// A public Load method for re-using same instance for multiple jobs
        /// </summary>
        /// <param name="pathToMap">A path to the source image</param>
        public byte[,] Load(string pathToMap)
        {
            _pathToMap = pathToMap;
            return Load();
        }

        private byte[,] Load()
        {
            if (String.IsNullOrEmpty(_pathToMap))
            {    
                throw new Exception("Attempted to load a map without providing a path to the MapLoader");
            }

            Bitmap bitmap = new Bitmap(_pathToMap);

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    byte avg = (byte)((bitmap.GetPixel(x, y).R + bitmap.GetPixel(x, y).G + bitmap.GetPixel(x, y).B) / 3);

                    _map[x, y] = avg;
                }
            }

            return _map;
        }
    }
}