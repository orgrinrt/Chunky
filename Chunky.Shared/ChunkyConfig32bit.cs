using System.Drawing.Imaging;

namespace Chunky.Shared
{
    public struct ChunkyConfig32bit
    {
        public ChunkyConfig32bit(
            string name, 
            string targetDir, 
            string sourcePath, 
            string targetImageType, 
            PixelFormat targetPixelFormat, 
            bool generateReconstruction, 
            bool generateVarianceComparison, 
            int chunkWidth, 
            int chunkHeight, 
            short chunkCountX, 
            short chunkCountY, 
            byte minDepth, 
            byte maxDepth)
        {
            Name = name;
            TargetDir = targetDir;
            SourcePath = sourcePath;
            TargetImageType = targetImageType;
            TargetPixelFormat = targetPixelFormat;
            GenerateReconstruction = generateReconstruction;
            GenerateVarianceComparison = generateVarianceComparison;
            ChunkWidth = chunkWidth;
            ChunkHeight = chunkHeight;
            ChunkCountX = chunkCountX;
            ChunkCountY = chunkCountY;
            MinDepth = minDepth;
            MaxDepth = maxDepth;
        }

        public ChunkyConfig32bit(
            string sourcePath, 
            bool generateReconstruction = true, 
            bool generateVarianceComparison = true) 
            : this(
                null,
                null,
                sourcePath,
                null,
                default,
                generateReconstruction,
                generateVarianceComparison,
                default,
                default,
                default,
                default,
                byte.MinValue,
                byte.MaxValue) {}
        
        public ChunkyConfig32bit(
            string sourcePath, 
            int chunkWidth,
            int chunkHeight,
            bool generateReconstruction = true, 
            bool generateVarianceComparison = true) 
            : this(
                null,
                null,
                sourcePath,
                null,
                default,
                generateReconstruction,
                generateVarianceComparison,
                chunkWidth,
                chunkHeight,
                default,
                default,
                byte.MinValue,
                byte.MaxValue) {}
        
        public ChunkyConfig32bit(
            string sourcePath, 
            short chunkCountX,
            short chunkCountY,
            bool generateReconstruction = true, 
            bool generateVarianceComparison = true) 
            : this(
                null,
                null,
                sourcePath,
                null,
                default,
                generateReconstruction,
                generateVarianceComparison,
                default,
                default,
                chunkCountX,
                chunkCountY,
                byte.MinValue,
                byte.MaxValue) {}
        
        /// <summary>
        /// If set, the result folder within TargetDir will be named after this, as well as all the resulting chunk files.
        /// Otherwise the name will be solved from SourcePath.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The target dir to which export the results.
        /// </summary>
        public string TargetDir { get; private set; }
        /// <summary>
        /// The path of the source bitmap.
        /// </summary>
        public string SourcePath { get; private set; }
        /// <summary>
        /// The target image type of the bitmap.
        /// Allowed values: png, jpg, bmp
        /// If not set, uses the same format as source.
        /// </summary>
        public string TargetImageType { get; private set; }
        /// <summary>
        /// The target pixel format of the bitmap.
        /// If not set, uses the same format as source.
        /// </summary>
        public PixelFormat TargetPixelFormat { get; private set; }
        
        /// <summary>
        /// Whether to generate a reconstruction.
        /// </summary>
        public bool GenerateReconstruction { get; private set; }
        /// <summary>
        /// Whether to generate a variance comparison result.
        /// </summary>
        public bool GenerateVarianceComparison { get; private set; }
        
        /// <summary>
        /// The width of each of the chunks. Overrides ChunkCountX if that is set in this config.
        /// </summary>
        public int ChunkWidth { get; private set; }
        /// <summary>
        /// The height of each of the chunks. Overrides ChunkCountY if that is set in this config.
        /// </summary>
        public int ChunkHeight { get; private set; }
        
        // Can't have both these and ChunkWidth/ChunkHeight. Width and Height override CountX and CountY.
        /// <summary>
        /// How many chunks to dedicate on the X axis.
        /// Note that if ChunkWidth value exists in this config, this value will be ignored.
        /// </summary>
        public short ChunkCountX { get; private set; }
        /// <summary>
        /// How many chunks to dedicate on the Y axis.
        /// Note that if ChunkHeight value exists in this config, this value will be ignored.
        /// </summary>
        public short ChunkCountY { get; private set; }
        
        /// <summary>
        /// Describes the minimum value of any given pixel channel
        /// </summary>
        public byte MinDepth { get; private set; }
        /// <summary>
        /// Describes the maximum value of any given pixel channel
        /// </summary>
        public byte MaxDepth { get; private set; }
    }
}