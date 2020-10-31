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
            string fileName = "testmap.png";
            string assetsDir = Path.Combine(Utils.SolveAssemblyRootDir(Assembly.GetCallingAssembly()), "Assets");
            string testMapPath = Path.Combine(assetsDir, fileName);
            Console.WriteLine(testMapPath);
            
            ChunkyController controller = new ChunkyController(new ChunkyConfig32bit(
                testMapPath,
                256,
                256,
                false,
                false));
            controller.Chunkify();
        }

        private static void ParseArgs(string[] args)
        {
            foreach (string arg in args)
            {
                
            }
        }
    }
}