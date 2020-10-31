using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using Chunky.Shared;
using Xunit;
using Xunit.Abstractions;

namespace Chunky.Tests
{
    public class ReconstructionAccuracy
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ReconstructionAccuracy(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void EnsurePixelAccuracyOfHeightMap256x256()
        {
            string assetsDir = Path.Combine(Utils.SolveAssemblyRootDir(Assembly.GetCallingAssembly()), "Assets");
            string testMapPath = Path.Combine(assetsDir, "testmap.png");
            _testOutputHelper.WriteLine(testMapPath);
            MapLoader loader = new MapLoader(testMapPath);
            Chonker chonker = new Chonker(loader.Map, 256, 256);
            Reconstruct(chonker.GenerateChunks(), Path.Combine(assetsDir, "Result"), "testmap", loader.OriginalBitmap);
        }
        
        private void Reconstruct(ChunkData[,] result, string targetDir, string name, Bitmap original)
        {
            Directory.CreateDirectory(targetDir);
            Reconstructor reconstructor = new Reconstructor(result, name, (short)original.Width, (short)original.Height);
            Bitmap[] results;

            results = reconstructor.ReconstructAndCompare(original, targetDir);
                
            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color originalPixel = original.GetPixel(x, y);
                    Color reconstructedPixel = results[0].GetPixel(x, y);

                    Assert.Equal(originalPixel, reconstructedPixel);
                }
            }

            Assert.Equal(original.Width, results[0].Width); // reconstruction
            Assert.Equal(original.Width, results[1].Width); // varianceComparison
        }

        [Fact]
        public void EnsurePixelAccuracyOfNormalImage100x200()
        {
            string assetsDir = Path.Combine(Utils.SolveAssemblyRootDir(Assembly.GetCallingAssembly()), "Assets");
            string testMapPath = Path.Combine(assetsDir, "colorscene.png");
            _testOutputHelper.WriteLine(testMapPath);
            MapLoader loader = new MapLoader(testMapPath);
            Chonker chonker = new Chonker(loader.Map, 100, 200);
            Reconstruct(chonker.GenerateChunks(), Path.Combine(assetsDir, "Result"), "colorscene", loader.OriginalBitmap);
        }
        
        [Fact]
        public void EnsurePixelAccuracyOfNormalImage99x138()
        {
            string assetsDir = Path.Combine(Utils.SolveAssemblyRootDir(Assembly.GetCallingAssembly()), "Assets");
            string testMapPath = Path.Combine(assetsDir, "colorscene.png");
            _testOutputHelper.WriteLine(testMapPath);
            
            MapLoader loader = new MapLoader(testMapPath);
            Chonker chonker = new Chonker(loader.Map, 100, 200);
            Reconstruct(chonker.GenerateChunks(), Path.Combine(assetsDir, "Result"), "colorscene", loader.OriginalBitmap);
        }
        
        [Fact]
        public void EnsurePixelAccuracyOfNormalImage444x18()
        {
            string assetsDir = Path.Combine(Utils.SolveAssemblyRootDir(Assembly.GetCallingAssembly()), "Assets");
            string testMapPath = Path.Combine(assetsDir, "colorscene.png");
            _testOutputHelper.WriteLine(testMapPath);
            
            MapLoader loader = new MapLoader(testMapPath);
            Chonker chonker = new Chonker(loader.Map, 444, 18);
            Reconstruct(chonker.GenerateChunks(), Path.Combine(assetsDir, "Result"), "colorscene", loader.OriginalBitmap);
        }
    }
}