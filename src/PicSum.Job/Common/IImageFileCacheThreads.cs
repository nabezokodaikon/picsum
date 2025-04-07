namespace PicSum.Job.Common
{
    public interface IImageFileCacheThreads
        : IDisposable
    {
        public void DoCache(string[] files);
    }
}
