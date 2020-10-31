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
            InitCliEnv();
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

        private static void InitCliEnv()
        {
            _cli.AddAction(
                "addSource", 
                "Specifies the source image for the operation. " +
                "Can be either a file for single operation or a directory for a batch operation.", 
                param => { _sourcePath = param; });
            _cli.AddCommand("-s", "addSource");
            _cli.AddCommand("--source", "addSource");

            _cli.AddAction(
                "config",
                "Specifies the config to use for the operation(s)." +
                "Can be a full path or a relative path",
                param => { _currConfig = State.LoadConfig(param); });
            _cli.AddCommand("-c", "config");
            _cli.AddCommand("--config", "config");
        }
    }
}