using System.Drawing.Imaging;

namespace Chunky.Shared
{
    public struct ChunkyConfig
    {
        public ChunkyConfig(string sourcePath) : this()
        {
            SourcePath = sourcePath;
        }

        /// <summary>
        /// If set, the result folder within TargetDir will be named after this, as well as all the resulting chunk files.
        /// Otherwise the name will be solved from SourcePath.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The target dir to which export the results.
        /// </summary>
        public string TargetDir { get; set; }
        /// <summary>
        /// The path of the source bitmap.
        /// </summary>
        public string SourcePath { get; set; }
        /// <summary>
        /// The target image type of the bitmap.
        /// Allowed values: png, jpg, bmp
        /// If not set, uses the same format as source.
        /// </summary>
        public string TargetImageType { get; set; }
        /// <summary>
        /// The target pixel format of the bitmap.
        /// If not set, uses the same format as source.
        /// </summary>
        public PixelFormat TargetPixelFormat { get; set; }
        
        /// <summary>
        /// The width of each of the chunks. Overrides ChunkCountX if that is set in this config.
        /// </summary>
        public short ChunkWidth { get; set; }
        /// <summary>
        /// The height of each of the chunks. Overrides ChunkCountY if that is set in this config.
        /// </summary>
        public short ChunkHeight { get; set; }
        
        // Can't have both these and ChunkWidth/ChunkHeight. Width and Height override CountX and CountY.
        /// <summary>
        /// How many chunks to dedicate on the X axis.
        /// Note that if ChunkWidth value exists in this config, this value will be ignored.
        /// </summary>
        public short ChunkCountX { get; set; }
        /// <summary>
        /// How many chunks to dedicate on the Y axis.
        /// Note that if ChunkHeight value exists in this config, this value will be ignored.
        /// </summary>
        public short ChunkCountY { get; set; }
        
        /// <summary>
        /// Describes the minimum value of any given pixel channel
        /// </summary>
        public byte MinDepth { get; set; }
        /// <summary>
        /// Describes the maximum value of any given pixel channel
        /// </summary>
        public byte MaxDepth { get; set; }
    }
}