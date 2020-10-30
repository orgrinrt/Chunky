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
        private readonly string _name;
        private readonly string _targetDir;
        private readonly Bitmap _originalBitmap;
        private ChunkData[,] _result;
        
        public string Name => _name;
        public string TargetDir => _targetDir;
        public Bitmap OriginalBitmap => _originalBitmap;
        public ChunkData[,] Result => _result;

        public ChunkyController(Bitmap originalBitmap, string name, string targetDir)
        {
            _name = name;
            _targetDir = targetDir;
            _originalBitmap = originalBitmap;
        }

        public ChunkyController(string pathToOriginalBitmap, string name = null, string targetDir = null)
        {
            _name = name ?? Utils.SolveNameFromFileName(pathToOriginalBitmap);
            _targetDir = targetDir ?? Path.Combine(Utils.SolveDirFromFileName(pathToOriginalBitmap), Name);
            _originalBitmap = new Bitmap(pathToOriginalBitmap);
        }

        public void SaveResultToDisk(string targetDir = null, string name = null)
        {
            name ??= Name;
            targetDir ??= TargetDir;
            
            string path = Path.Combine(targetDir, name);
            
            Directory.CreateDirectory(path);
            ChunkData chunk;

            for (int x = 0; x < Result.GetLength(0); x++)
            {
                for (int y = 0; y < Result.GetLength(1); y++)
                {
                    chunk = Result[x,y];
                        
                    chunk.Bitmap.Save(Path.Combine(path, name + "-" + chunk.X + "-" + chunk.Y + ".png"), ImageFormat.Png);
                }
            }

            Reconstructor reconstructor = new Reconstructor(Result, name, (short)OriginalBitmap.Width, (short)OriginalBitmap.Height);
            
            reconstructor.ReconstructAndCompare(OriginalBitmap, path);
            //reconstructor.Reconstruct(targetDir);
        }
    }
}