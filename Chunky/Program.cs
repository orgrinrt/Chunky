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
        private static ChunkyConfig32bit _currConfig;
        
        private static string _sourcePathOverride;
        private static string _chunkWidthOverride;
        private static string _chunkHeightOverride;
        private static string _chunkCountXOverride;
        private static string _chunkCountYOverride;
        private static string _compatibilityModeOverride;
        
        private static CommandLineController _cli = new CommandLineController(
            "Chunky", 
            "A simple program to turn images into chunks.");
        static void Main(string[] args)
        {
            InitCliEnv();
            _currConfig = State.LoadDefaultConfig();
            ParseArgs(args);
            _currConfig.SourcePath = _sourcePathOverride;
            Print.Line(ConsoleColor.Green, "Target dir: " + _currConfig.TargetDir);
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
            
            ChunkyBatchProcessor batcher = new ChunkyBatchProcessor(_currConfig.ParsePaths());
            batcher.Chunkify();
            
        }

        private static void ParseArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i += 2)
            {
                if (_cli.Commands.ContainsKey(args[i]))
                {
                    if (args.Length > i + 1)
                    {
                        _cli.Execute(args[i], args[i + 1]);
                    }
                    else
                    {
                        _cli.Execute(args[i], "");
                    }
                }
                else if (args[i].StartsWith('-'))
                {
                    // this is a shorthand combination of flags/params likely
                    // TODO: Handle a combination of commands
                }
            }
        }

        private static void InitCliEnv()
        {
            _cli.AddAction(
                "addSource", 
                "Specifies the source image for the operation. " +
                "Can be either a file for single operation or a directory for a batch operation.",
                param =>
                {
                    
                    _sourcePathOverride = Utils.ParsePath(param);
                });
            _cli.AddCommand("-s", "addSource");
            _cli.AddCommand("--source", "addSource");

            _cli.AddAction(
                "config",
                "Specifies the config to use for the operation(s). " +
                "Can be a full path or a relative path",
                param =>
                {
                    Print.Line(ConsoleColor.White, "\t Overwrote config with one from " + param);
                    _currConfig = State.LoadConfig(Utils.ParsePath(param));
                });
            _cli.AddCommand("-c", "config");
            _cli.AddCommand("--config", "config");
        }
    }
}