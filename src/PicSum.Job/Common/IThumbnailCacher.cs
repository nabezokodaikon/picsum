using PicSum.Job.Entities;

namespace PicSum.Job.Common
{
    public interface IThumbnailCacher
        : IDisposable
    {
        public ValueTask<ThumbnailCacheEntity> GetCache(string filePath);
        public ValueTask<ThumbnailCacheEntity> GetOrCreateCache(string filePath, int thumbWidth, int thumbHeight);
    }
}
