using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

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
        private readonly ChunkyConfig _config;
        private readonly Bitmap _originalBitmap;
        private readonly PixelFormat _originalPixelFormat;
        private ChunkData[,] _result;

        public ChunkyConfig Config => _config;
        public Bitmap OriginalBitmap => _originalBitmap;
        public ChunkData[,] Result => _result;
        
        public ChunkyController(ChunkyConfig config)
        {
            string name = "";
            string targetDir = "";
            
            if (string.IsNullOrEmpty(config.SourcePath)) throw new Exception("Attempted to pass a faulty config (source path missing)");
            if (string.IsNullOrEmpty(config.Name)) name = Utils.SolveNameFromFileName(config.SourcePath);
            if (string.IsNullOrEmpty(config.TargetDir)) targetDir = Path.Combine(Utils.SolveDirFromFileName(config.SourcePath), name);

            _originalBitmap = new Bitmap(config.SourcePath);
            _originalPixelFormat = _originalBitmap.PixelFormat;

            _config = new ChunkyConfig
            {
                SourcePath = config.SourcePath,
                Name = name,
                TargetDir = targetDir,
                TargetImageType = config.TargetImageType ?? Utils.SolveImageExtensionFromFileName(config.SourcePath),
                TargetPixelFormat = config.TargetPixelFormat != default ? config.TargetPixelFormat : _originalPixelFormat
            };
        }

        public ChunkData[,] Chunkify(bool saveToDisk = true)
        {
            MapLoader loader = new MapLoader(Config.SourcePath);
            Chonker chonker;
            // we always use the width & height when available (i.e not default (=0))
            if (Config.ChunkWidth != default && Config.ChunkHeight != default)
            {
                chonker = new Chonker(loader.Map, (int)Config.ChunkWidth, (int)Config.ChunkHeight);
            }
            else if (Config.ChunkWidth != default)
            {
                chonker = new Chonker(loader.Map, (int)Config.ChunkWidth, Config.ChunkCountY);
            }
            else if (Config.ChunkHeight != default)
            {
                chonker = new Chonker(loader.Map, Config.ChunkCountX, (int)Config.ChunkHeight);
            }
            else
            {
                chonker = new Chonker(loader.Map, Config.ChunkCountX, Config.ChunkCountY);
            }
            
            _result = chonker.GenerateChunks();
            if (saveToDisk) SaveResultToDisk();
            return Result;
        }

        public void SaveResultToDisk(bool exportReconstruction = true, bool exportVarianceTest = true, string targetDir = null, string name = null)
        {
            name ??= Config.Name;
            targetDir ??= Config.TargetDir;
            
            string path = Path.Combine(targetDir, name);
            
            Directory.CreateDirectory(path);

            for (int x = 0; x < Result.GetLength(0); x++)
            {
                for (int y = 0; y < Result.GetLength(1); y++)
                {
                    ChunkData chunk = Result[x,y];

                    chunk.Bitmap.Save(Path.Combine(path, name + "-" + chunk.X + "-" + chunk.Y + ".png"), ImageFormat.Png);
                }
            }

            if (exportReconstruction)
            {
                Reconstructor reconstructor = new Reconstructor(Result, name, (short)OriginalBitmap.Width, (short)OriginalBitmap.Height);
                if (exportVarianceTest) reconstructor.ReconstructAndCompare(OriginalBitmap, path);
                else reconstructor.Reconstruct(targetDir);
            }
        }
    }
}