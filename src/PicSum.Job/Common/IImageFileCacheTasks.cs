namespace PicSum.Job.Common
{
    public interface IImageFileCacheTasks
        : IDisposable
    {
        public void DoCache(string[] files);
    }
}
