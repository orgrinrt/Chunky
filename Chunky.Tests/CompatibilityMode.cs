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
        public void ExperimentalModeIsFaster()
        {
            string fileName = "testmap.png";
            string assetsDir = Path.Combine(Utils.SolveAssemblyRootDir(Assembly.GetCallingAssembly()), "Assets");
            string testMapPath = Path.Combine(assetsDir, fileName);
            _testOutputHelper.WriteLine(testMapPath);
            
            Stopwatch sw = new Stopwatch();
            long nonThreadedDuration, threadedDuration;
            
            // Let's do non-threaded first
            
            sw.Start();
            ChunkyController controllerA = new ChunkyController(new ChunkyConfig32bit(
                testMapPath,
                256,
                256,
                true,
                false,
                false));
            controllerA.Chunkify();
            nonThreadedDuration = sw.ElapsedMilliseconds;
            sw.Stop();
            sw.Reset();
            
            // And then threaded
            
            sw.Start();
            ChunkyController controllerB = new ChunkyController(new ChunkyConfig32bit(
                testMapPath,
                256,
                256,
                false,
                false,
                false));
            controllerB.Chunkify();
            threadedDuration = sw.ElapsedMilliseconds;
            sw.Stop();

            _testOutputHelper.WriteLine("Non threaded duration: " + nonThreadedDuration);
            _testOutputHelper.WriteLine("Threaded duration: " + threadedDuration);
            Assert.True(threadedDuration < nonThreadedDuration);
        }
    }
}