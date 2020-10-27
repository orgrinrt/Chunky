using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Chunky.Shared
{
    public class MapLoader
    {
        private string _pathToMap;
        private Bitmap _originalBitmap;
        private byte[,] _map;

        
        public Bitmap OriginalBitmap => _originalBitmap;
        public byte[,] Map => _map;

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

            Console.WriteLine("PATH TO MAP: " + _pathToMap);
            Bitmap bitmap = new Bitmap(_pathToMap);
            bitmap.Save("Q:/test1.png");
            _originalBitmap = bitmap;
            
            Bitmap newBmp = new Bitmap(Image.FromFile(_pathToMap));

            bitmap = newBmp.Clone(
                new Rectangle(0, 0, newBmp.Width, newBmp.Height),
                PixelFormat.Format32bppRgb);
            
            bitmap.Save("Q:/test2.png");

            //Console.WriteLine(bitmap.GetPixel(600, 1250).R);

            //throw new Exception();
            _map = new byte[bitmap.Width, bitmap.Height];

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    int avg = ((bitmap.GetPixel(x, y).R + bitmap.GetPixel(x, y).G + bitmap.GetPixel(x, y).B) / 3);

                    //Console.WriteLine(avg);
                    //Console.WriteLine("R: " + bitmap.GetPixel(x, y).R);
                    //Console.WriteLine("G: " + bitmap.GetPixel(x, y).G);
                    //Console.WriteLine("B: " + bitmap.GetPixel(x, y).B);
                    //Console.WriteLine("----------");
                    //Console.WriteLine(bitmap.GetPixel(x, y).R);
                    _map[x, y] = bitmap.GetPixel(x, y).R;
                }
            }

            return _map;
        }
    }
}