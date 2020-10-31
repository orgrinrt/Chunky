namespace Chunky.Shared
{
    public class GlobalConfig
    {
        public GlobalConfig(int parallelProcessCount)
        {
            ParallelProcessCount = parallelProcessCount;
        }

        public GlobalConfig()
        {
            ParallelProcessCount = 10;
        }

        public int ParallelProcessCount { get; set; }
    }
}