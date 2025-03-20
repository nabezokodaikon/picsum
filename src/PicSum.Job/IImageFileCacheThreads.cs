namespace PicSum.Job
{
    public interface IImageFileCacheThreads
        : IDisposable
    {
        public void DoCache(string[] files);
    }
}
