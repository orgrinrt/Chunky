using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Chunky.Shared
{
    public class ChunkyBatchProcessor
    {
        private ChunkyConfig32bit _config;
        private ConcurrentQueue<ChunkyProcessor> _processors = new ConcurrentQueue<ChunkyProcessor>();
        private ChunkData[][,] _result;
        
        public ChunkyConfig32bit Config => _config;
        public ChunkData[][,] Result => _result;

        public ChunkyBatchProcessor(ChunkyConfig32bit config)
        {
            _config = config;

            if (!Directory.Exists(config.SourcePath)) throw new Exception("Attempted to batch operate on a path but it didn't exist");

            string[] files = Directory.GetFiles(config.SourcePath);

            foreach (string filePath in files)
            {
                _processors.Enqueue(new ChunkyProcessor(new ChunkyConfig32bit(
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
            int currProcessing = 0;
            object countLock = new object();
            object currProcessingLock = new object();

            while (_processors.Count > 0)
            {
                if (Chunky.Config.ParallelProcessCount < currProcessing) continue;
                
                if (!Config.CompatibilityMode)
                {
                    Thread thread = new Thread(() =>
                    {
                        if (_processors.TryDequeue(out ChunkyProcessor processor))
                        {
                            lock (currProcessingLock) currProcessing++;
                            result.Add(processor.Chunkify(saveToDisk));
                            lock (countLock) processedCount++;
                            lock (currProcessingLock) currProcessing--;
                        }
                    });
                    thread.Start();
                }
                else
                {
                    if (_processors.TryDequeue(out ChunkyProcessor processor))
                    {
                        result.Add(processor.Chunkify(saveToDisk));
                        processedCount++;
                    }
                }
            }
            
            while (processedCount < _processors.Count) { }

            _result = result.ToArray();
            return _result;
        }
    }
}