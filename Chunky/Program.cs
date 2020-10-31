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
        private static string _sourcePath;
        private static ChunkyConfig32bit _currConfig;
        private static CommandLineController _cli = new CommandLineController(
            "Chunky", 
            "A simple program to turn images into chunks.");
        static void Main(string[] args)
        {
            ParseArgs(args);
            Console.ReadLine();
            /*
            string fileName = "colorscene.png";
            string assetsDir = Path.Combine(Utils.SolveAssemblyRootDir(Assembly.GetCallingAssembly()), "Assets");
            string testMapPath = Path.Combine(assetsDir, fileName);
            Console.WriteLine(testMapPath);
            */
            /*
            ChunkyProcessor processor = new ChunkyProcessor(new ChunkyConfig32bit(
                testMapPath,
                (short)10,
                (short)10,
                false,
                true,
                true));
            processor.Chunkify();
            */
            /*
            ChunkyBatchProcessor batcher = new ChunkyBatchProcessor(new ChunkyConfig32bit(
                assetsDir,
                (short)10,
                (short)10,
                false,
                true,
                true));
            batcher.Chunkify();
            */
        }

        private static void ParseArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i += 2)
            {
                if (_cli.Commands.ContainsKey(args[i]))
                {
                    if (args.Length > i + 2)
                    {
                        _cli.Actions[_cli.Commands[args[i]]](args[i + 1]);
                    }
                    else
                    {
                        _cli.Actions[_cli.Commands[args[i]]]("");
                    }
                }
            }
        }
    }
}