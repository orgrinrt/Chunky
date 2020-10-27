using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using Chunky.Shared;

namespace Chunky
{
    class Program
    {
        static void Main(string[] args)
        {
            string dllName = Assembly.GetCallingAssembly().GetName().ToString().Split(',')[0] + ".dll";
            string rootDir = Path.Combine(Assembly.GetCallingAssembly().Location
                .Remove(Assembly.GetCallingAssembly().Location.Length - dllName.Length));
            string assetsDir = Path.Combine(rootDir, "Assets");
            string testMapPath = Path.Combine(assetsDir, "testmap.png");
            Console.WriteLine(testMapPath);
            MapLoader loader = new MapLoader(testMapPath);
            Chonker chonker = new Chonker(loader.Map, 256, 256);
            SaveResultToDisk(chonker.GenerateChunks(), Path.Combine(assetsDir, "Result"), "testmap");
        }

        private static void SaveResultToDisk(ChunkData[,] result, string targetDir, string name)
        {
            Directory.CreateDirectory(targetDir);
            ChunkData chunk;

            for (int x = 0; x < result.GetLength(0); x++)
            {
                for (int y = 0; y < result.GetLength(1); y++)
                {
                    chunk = result[x,y];
                        
                    chunk.Bitmap.Save(Path.Combine(targetDir, name + "-" + chunk.X + "-" + chunk.Y + ".png"), ImageFormat.Png);
                    //Console.WriteLine(chunk.Bitmap.GetPixel(0 + offset, 75));
                }
            }

            Reconstructor reconstructor = new Reconstructor(result);
            reconstructor.Reconstruct(Path.Combine(targetDir, name + "-" + "reconstruct.png"));
        }

        private static void ParseArgs(string[] args)
        {
            foreach (string arg in args)
            {
                
            }
        }
    }
}