using SWF.Core.Base;
using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ImageFileTakenDateCacher
        : IImageFileTakenDateCacher
    {
        private const int CACHE_CAPACITY = 10000;

        private bool _disposed = false;
        private readonly Dictionary<string, ImageFileTakenDateCacheEntity> _cacheDictionary = new(CACHE_CAPACITY);
        private readonly Lock _cacheLock = new();

        public ImageFileTakenDateCacher()
        {

        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {

            }

            this._disposed = true;
        }

        public DateTime Get(string filePath)
        {
            lock (this._cacheLock)
            {
                if (this._cacheDictionary.TryGetValue(filePath, out var cache))
                {
                    var updateDate = FileUtil.GetUpdateDate(filePath);
                    if (cache.UpdateDate == updateDate)
                    {
                        return cache.TakenDate;
                    }
                }

                return ImageFileTakenDateCacheEntity.EMPTY.TakenDate;
            }
        }

        public DateTime GetOrCreate(string filePath)
        {
            if (!ImageUtil.CanRetainExifImageFormat(filePath))
            {
                return DateTimeExtensions.EMPTY;
            }

            lock (this._cacheLock)
            {
                if (this._cacheDictionary.TryGetValue(filePath, out var cache))
                {
                    var updateDate = FileUtil.GetUpdateDate(filePath);
                    if (cache.UpdateDate == updateDate)
                    {
                        return cache.TakenDate;
                    }

                    this._cacheDictionary.Remove(filePath);
                }
            }

            var newCache = new ImageFileTakenDateCacheEntity(
                filePath,
                ImageUtil.GetTakenDate(filePath),
                FileUtil.GetUpdateDate(filePath));

            lock (this._cacheLock)
            {
                if (this._cacheDictionary.TryGetValue(filePath, out var cache))
                {
                    var updateDate = FileUtil.GetUpdateDate(filePath);
                    if (cache.UpdateDate == updateDate)
                    {
                        return cache.TakenDate;
                    }

                    this._cacheDictionary.Remove(filePath);
                }

                this._cacheDictionary.Add(filePath, newCache);

                return newCache.TakenDate;
            }
        }
    }
}
