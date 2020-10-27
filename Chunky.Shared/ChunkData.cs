using System.Drawing;

namespace Chunky.Shared
{
    public struct ChunkData
    {
        public Bitmap Bitmap { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public ChunkData(Bitmap bitmap, int x, int y)
        {
            Bitmap = bitmap;
            X = x;
            Y = y;
        }
    }
}