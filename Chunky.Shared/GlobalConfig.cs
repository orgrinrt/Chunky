namespace Chunky.Shared
{
    public struct GlobalConfig
    {
        public GlobalConfig(int parallelProcessCount = 10, bool verbose = false)
        {
            ParallelProcessCount = parallelProcessCount;
            Verbose = verbose;
        }

        public int ParallelProcessCount { get; set; }
        public bool Verbose { get; set; }
    }
}