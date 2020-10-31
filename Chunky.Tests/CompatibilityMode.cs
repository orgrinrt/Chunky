using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Chunky.Shared;
using Xunit;
using Xunit.Abstractions;

namespace Chunky.Tests
{
    public class CompatibilityMode
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public CompatibilityMode(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ExperimentalChonkingIsFaster()
        {
            ColorRgba32bit[,] map = new ColorRgba32bit[10000,10000];
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    map[x, y] = new ColorRgba32bit(255, 255, 255, 255);
                }
            }
            
            Chonker chonker = new Chonker(map, 100, 100);
            long nonThreadedDuration, threadedDuration;
            
            Stopwatch sw = new Stopwatch();
            sw.Start();
            chonker.GenerateChunks(true);
            nonThreadedDuration = sw.ElapsedMilliseconds;
            sw.Stop();
            sw.Reset();
            
            sw.Start();
            chonker.GenerateChunks(false);
            threadedDuration = sw.ElapsedMilliseconds;
            sw.Stop();
            sw.Reset();

            _testOutputHelper.WriteLine("Non threaded duration: " + nonThreadedDuration);
            _testOutputHelper.WriteLine("Threaded duration: " + threadedDuration);
            Assert.True(nonThreadedDuration > threadedDuration);
        }
        
        [Fact]
        public void ExperimentalModeDoesntCrashAndIsFaster()
        {
            string fileName = "testmap.png";
            string assetsDir = Path.Combine(Utils.SolveAssemblyRootDir(Assembly.GetCallingAssembly()), "Assets");
            string testMapPath = Path.Combine(assetsDir, fileName);
            _testOutputHelper.WriteLine(testMapPath);
            
            Stopwatch sw = new Stopwatch();
            long nonThreadedDuration, threadedDuration;
            
            // Let's do non-threaded first
            
            sw.Start();
            ChunkyProcessor processorA = new ChunkyProcessor(new ChunkyConfig32bit(
                testMapPath,
                256,
                256,
                true,
                false,
                false));
            processorA.Chunkify();
            nonThreadedDuration = sw.ElapsedMilliseconds;
            sw.Stop();
            sw.Reset();
            
            // And then threaded
            
            sw.Start();
            ChunkyProcessor processorB = new ChunkyProcessor(new ChunkyConfig32bit(
                testMapPath,
                256,
                256,
                false,
                false,
                false));
            processorB.Chunkify();
            threadedDuration = sw.ElapsedMilliseconds;
            sw.Stop();

            _testOutputHelper.WriteLine("Non threaded duration: " + nonThreadedDuration);
            _testOutputHelper.WriteLine("Threaded duration: " + threadedDuration);
            Assert.True(threadedDuration < nonThreadedDuration);
            // if we got here then it didn't crash either
        }
    }
}