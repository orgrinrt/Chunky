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
            string fileName = "colorscene.png";
            string assetsDir = Path.Combine(Utils.SolveAssemblyRootDir(Assembly.GetCallingAssembly()), "Assets");
            string testMapPath = Path.Combine(assetsDir, fileName);
            Console.WriteLine(testMapPath);
            MapLoader loader = new MapLoader(testMapPath);
            Chonker chonker = new Chonker(loader.Map, 410, 225);
            SaveResultToDisk(chonker.GenerateChunks(), Path.Combine(assetsDir, "Results"), Utils.SolveNameFromFileName(fileName), loader.OriginalBitmap);
        }

        private static void SaveResultToDisk(ChunkData[,] result, string targetDir, string name, Bitmap original)
        {
            string path = Path.Combine(targetDir, name);
            
            Directory.CreateDirectory(path);
            ChunkData chunk;

            for (int x = 0; x < result.GetLength(0); x++)
            {
                for (int y = 0; y < result.GetLength(1); y++)
                {
                    chunk = result[x,y];
                        
                    chunk.Bitmap.Save(Path.Combine(path, name + "-" + chunk.X + "-" + chunk.Y + ".png"), ImageFormat.Png);
                    //Console.WriteLine(chunk.Bitmap.GetPixel(0 + offset, 75));
                }
            }

            Reconstructor reconstructor = new Reconstructor(result, name, (short)original.Width, (short)original.Height);
            
            reconstructor.ReconstructAndCompare(original, path);
            //reconstructor.Reconstruct(targetDir);
        }

        private static void ParseArgs(string[] args)
        {
            foreach (string arg in args)
            {
                
            }
        }
    }
}