using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ImageFileTakenCacher
        : IImageFileTakenCacher
    {
        private bool _disposed = false;
        private readonly Dictionary<string, ImageFileTakenCacheEntity> _cacheDictionary = [];
        private readonly Lock _cacheLock = new();

        public ImageFileTakenCacher()
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

                return ImageFileTakenCacheEntity.EMPTY.TakenDate;
            }
        }

        public DateTime GetOrCreate(string filePath)
        {
            if (!ImageUtil.CanRetainExifImageFormat(filePath))
            {
                return FileUtil.EMPTY_DATETIME;
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

            var newCache = new ImageFileTakenCacheEntity(
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
