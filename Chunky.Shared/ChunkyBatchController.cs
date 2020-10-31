using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Chunky.Shared
{
    public class ChunkyBatchController
    {
        private ChunkyConfig32bit _config;
        private List<ChunkyController> _controllers = new List<ChunkyController>();
        private ChunkData[][,] _result;
        
        public ChunkyConfig32bit Config => _config;
        public ChunkData[][,] Result => _result;

        public ChunkyBatchController(ChunkyConfig32bit config)
        {
            _config = config;

            if (!Directory.Exists(config.SourcePath)) throw new Exception("Attempted to batch operate on a path but it didn't exist");

            string[] files = Directory.GetFiles(config.SourcePath);

            foreach (string filePath in files)
            {
                _controllers.Add(new ChunkyController(new ChunkyConfig32bit(
                    config.Name,
                    config.TargetDir,
                    filePath,
                    config.TargetImageType,
                    config.TargetPixelFormat,
                    config.CompatibilityMode,
                    config.GenerateReconstruction,
                    config.GenerateVarianceComparison,
                    config.ChunkWidth,
                    config.ChunkHeight,
                    config.ChunkCountX,
                    config.ChunkCountY,
                    config.MinDepth,
                    config.MaxDepth)));
            }
        }

        public ChunkData[][,] Chunkify(bool saveToDisk = true)
        {
            BlockingCollection<ChunkData[,]> result = new BlockingCollection<ChunkData[,]>();
            int processedCount = 0;
            object countLock = new object();

            foreach (ChunkyController controller in _controllers)
            {
                if (!Config.CompatibilityMode)
                {
                    Thread thread = new Thread(() =>
                    {
                        result.Add(controller.Chunkify(saveToDisk));
                        lock (countLock) processedCount++;
                    });
                    thread.Start();
                }
                else
                {
                    result.Add(controller.Chunkify(saveToDisk));
                    processedCount++;
                }
            }
            
            while (processedCount < _controllers.Count) { }

            _result = result.ToArray();
            return _result;
        }
    }
}