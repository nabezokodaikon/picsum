using PicSum.Job.Entities;

namespace PicSum.Job.Common
{
    public interface IThumbnailCacher
        : IDisposable
    {
        public ThumbnailCacheEntity GetOnlyCache(string filePath, int thumbWidth, int thumbHeight);
        public ThumbnailCacheEntity GetOrCreateCache(string filePath, int thumbWidth, int thumbHeight);
    }
}
