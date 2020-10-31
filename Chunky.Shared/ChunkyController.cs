using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

namespace Chunky.Shared
{
    /// <summary>
    /// Controls all chunking behaviour for a single bitmap instance.
    /// For batch operations etc. use multiple controllers.
    /// </summary>
    public class ChunkyController
    {
        // immutable by design, to allow for safer multi-threading
        // for new images/batch operations, allocate new ones or replace with new instance
        private readonly ChunkyConfig32bit _config;
        private readonly Bitmap _originalBitmap;
        private readonly PixelFormat _originalPixelFormat;
        private ChunkData[,] _result;

        public ChunkyConfig32bit Config => _config;
        public Bitmap OriginalBitmap => _originalBitmap;
        public ChunkData[,] Result => _result;
        
        public ChunkyController(ChunkyConfig32bit config)
        {
            string name = "";
            string targetDir = "";
            
            if (string.IsNullOrEmpty(config.SourcePath)) throw new Exception("Attempted to pass a faulty config (source path missing)");
            if (string.IsNullOrEmpty(config.Name)) name = Utils.SolveNameFromFileName(config.SourcePath);
            if (string.IsNullOrEmpty(config.TargetDir)) targetDir = Path.Combine(Utils.SolveDirFromFileName(config.SourcePath), "Results");

            _originalBitmap = new Bitmap(config.SourcePath);
            _originalPixelFormat = _originalBitmap.PixelFormat;

            _config = new ChunkyConfig32bit(
                name,
                targetDir,
                config.SourcePath,
                config.TargetImageType ?? Utils.SolveImageExtensionFromFileName(config.SourcePath),
                config.TargetPixelFormat != default ? config.TargetPixelFormat : _originalPixelFormat,
                config.CompatibilityMode,
                config.GenerateReconstruction,
                config.GenerateVarianceComparison,
                config.ChunkWidth,
                config.ChunkHeight,
                config.ChunkCountX,
                config.ChunkCountY,
                config.MinDepth,
                config.MaxDepth
            );
        }

        public ChunkData[,] Chunkify(bool saveToDisk = true)
        {
            MapLoader loader = new MapLoader(Config.SourcePath);
            Chonker chonker;
            // we always use the width & height when available (i.e not default (=0))
            if (Config.ChunkWidth != default && Config.ChunkHeight != default)
            {
                //Console.WriteLine("CHONKER USES WIDTH & HEIGHT");
                chonker = new Chonker(loader.Map, Config.ChunkWidth, Config.ChunkHeight);
            }
            else if (Config.ChunkWidth != default)
            {
                //Console.WriteLine("CHONKER USES WIDTH & CHUNK COUNT Y");
                chonker = new Chonker(loader.Map, Config.ChunkWidth, Config.ChunkCountY);
            }
            else if (Config.ChunkHeight != default)
            {
                //Console.WriteLine("CHONKER USES CHUNK COUNT X & HEIGHT");
                chonker = new Chonker(loader.Map, Config.ChunkCountX, Config.ChunkHeight);
            }
            else
            {
                //Console.WriteLine("CHONKER USES CHUNK COUNT X & CHUNK COUNT Y");
                chonker = new Chonker(loader.Map, Config.ChunkCountX, Config.ChunkCountY);
            }
            
            _result = chonker.GenerateChunks(Config.CompatibilityMode);
            if (saveToDisk) SaveResultToDisk(Config.GenerateReconstruction, Config.GenerateVarianceComparison);
            return Result;
        }

        public void SaveResultToDisk(bool exportReconstruction = true, bool exportVarianceTest = true, string targetDir = null, string name = null)
        {
            name ??= Config.Name;
            targetDir ??= Config.TargetDir;

            string path = Path.Combine(targetDir, name);

            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);

                foreach (string filePath in files)
                {
                    File.Delete(filePath);
                }
            }
            Directory.CreateDirectory(path);

            for (int x = 0; x < Result.GetLength(0); x++)
            {
                for (int y = 0; y < Result.GetLength(1); y++)
                {
                    ChunkData chunk = Result[x,y];

                    if (!Config.CompatibilityMode)
                    {
                        Thread thread = new Thread(() =>
                            chunk.Bitmap.Save(Path.Combine(path, name + "-" + chunk.X + "-" + chunk.Y + ".png"),
                                ImageFormat.Png)
                        );
                        thread.Start();
                    }
                    else
                    {
                        chunk.Bitmap.Save(Path.Combine(path, name + "-" + chunk.X + "-" + chunk.Y + ".png"),
                            ImageFormat.Png);
                    }
                }
            }

            if (exportReconstruction)
            {
                //Reconstructor reconstructor = new Reconstructor(Result, name, (short)OriginalBitmap.Width, (short)OriginalBitmap.Height);
                Reconstructor reconstructor = new Reconstructor(path, name, (short)OriginalBitmap.Width, (short)OriginalBitmap.Height);
                if (exportVarianceTest) reconstructor.ReconstructAndCompare(OriginalBitmap, path, Config.CompatibilityMode);
                else reconstructor.Reconstruct(targetDir);
            }
        }
    }
}