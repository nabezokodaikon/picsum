using SWF.Core.Base;
using SWF.Core.FileAccessor;

namespace SWF.Core.ImageAccessor
{

    public sealed class ImageFileTakenDateCacher
        : IImageFileTakenDateCacher
    {
        private const int CACHE_CAPACITY = 10000;

        private bool _disposed = false;
        private readonly Dictionary<string, ImageFileTakenDateCacheEntity> _cacheDictionary = new(CACHE_CAPACITY);
        private readonly SemaphoreSlim _cacheLock = new(1, 1);

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
                this._cacheLock.Dispose();
            }

            this._disposed = true;
        }

        public DateTime Get(string filePath)
        {
            this._cacheLock.Wait();
            try
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
            finally
            {
                this._cacheLock.Release();
            }
        }

        public DateTime GetOrCreate(string filePath)
        {
            if (!ImageUtil.CanRetainExifImageFormat(filePath))
            {
                return DateTimeExtensions.EMPTY;
            }

            this._cacheLock.Wait();
            try
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
            finally
            {
                this._cacheLock.Release();
            }

            var newCache = new ImageFileTakenDateCacheEntity(
                filePath,
                ImageUtil.GetTakenDate(filePath),
                FileUtil.GetUpdateDate(filePath));

            this._cacheLock.Wait();
            try
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
            finally
            {
                this._cacheLock.Release();
            }
        }
    }
}
