﻿using System;
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
            string assetsDir = Path.Combine(Utils.SolveAssemblyRootDir(Assembly.GetCallingAssembly()), "Assets");
            string testMapPath = Path.Combine(assetsDir, "colorscene.png");
            Console.WriteLine(testMapPath);
            MapLoader loader = new MapLoader(testMapPath);
            Chonker chonker = new Chonker(loader.Map, 410, 225);
            SaveResultToDisk(chonker.GenerateChunks(), Path.Combine(assetsDir, "Result"), "testmap", loader.OriginalBitmap);
        }

        private static void SaveResultToDisk(ChunkData[,] result, string targetDir, string name, Bitmap original)
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

            Reconstructor reconstructor = new Reconstructor(result, name, (short)original.Width, (short)original.Height);
            
            reconstructor.ReconstructAndCompare(original, targetDir);
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