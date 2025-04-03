using PicSum.Job.Entities;

namespace PicSum.Job.Common
{
    public interface IThumbnailCacher
        : IDisposable
    {
        public ThumbnailCacheEntity GetCache(string filePath);
        public ThumbnailCacheEntity GetOrCreateCache(string filePath, int thumbWidth, int thumbHeight);
    }
}
