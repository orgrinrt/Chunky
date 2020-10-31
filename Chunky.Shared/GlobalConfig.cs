namespace Chunky.Shared
{
    public struct GlobalConfig
    {
        public GlobalConfig(int parallelProcessCount = 10)
        {
            ParallelProcessCount = parallelProcessCount;
        }

        public int ParallelProcessCount { get; set; }
    }
}