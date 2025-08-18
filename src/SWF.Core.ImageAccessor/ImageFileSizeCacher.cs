using SWF.Core.Base;
using SWF.Core.FileAccessor;

namespace SWF.Core.ImageAccessor
{

    public sealed partial class ImageFileSizeCacher
        : IImageFileSizeCacher
    {
        private const int CACHE_CAPACITY = 10000;

        private bool _disposed = false;
        private readonly Dictionary<string, ImageFileSizeCacheEntity> _cacheDictionary = new(CACHE_CAPACITY);
        private readonly Lock _cacheLock = new();

        public ImageFileSizeCacher()
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

        public void Create(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var updateDate = FileUtil.GetUpdateDate(filePath);

            lock (this._cacheLock)
            {
                using (TimeMeasuring.Run(false, $"ImageFileSizeCacher.Create 1"))
                {
                    if (this._cacheDictionary.TryGetValue(filePath, out var cache))
                    {
                        if (updateDate == cache.UpdateDate)
                        {
                            return;
                        }
                    }
                }
            }

            var newCache = new ImageFileSizeCacheEntity(
                filePath, ImageUtil.GetImageSize(filePath), updateDate);

            lock (this._cacheLock)
            {
                using (TimeMeasuring.Run(false, $"ImageFileSizeCacher.Create 2"))
                {
                    if (this._cacheDictionary.TryGetValue(filePath, out var cache))
                    {
                        if (newCache.UpdateDate == cache.UpdateDate)
                        {
                            return;
                        }

                        this._cacheDictionary.Remove(cache.FilePath);
                    }

                    this._cacheDictionary.Add(newCache.FilePath, newCache);
                }
            }
        }

        public ImageFileSizeCacheEntity GetOrCreate(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            using (TimeMeasuring.Run(false, "ImageFileSizeCacher.GetOrCreate"))
            {
                var updateDate = FileUtil.GetUpdateDate(filePath);

                lock (this._cacheLock)
                {
                    if (this._cacheDictionary.TryGetValue(filePath, out var cache))
                    {
                        if (updateDate == cache.UpdateDate)
                        {
                            return cache;
                        }
                    }
                }

                var size = ImageUtil.GetImageSize(filePath);
                this.Set(filePath, size, updateDate);
                return new ImageFileSizeCacheEntity(
                    filePath, size, updateDate);
            }
        }

        public void Set(string filePath, Size size, DateTime updateDate)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            lock (this._cacheLock)
            {
                using (TimeMeasuring.Run(false, $"ImageFileSizeCacher.Set 1"))
                {
                    if (this._cacheDictionary.TryGetValue(filePath, out var cache))
                    {
                        if (updateDate == cache.UpdateDate)
                        {
                            return;
                        }
                    }
                }
            }

            var newCache = new ImageFileSizeCacheEntity(
                filePath, size, updateDate);

            lock (this._cacheLock)
            {
                using (TimeMeasuring.Run(false, $"ImageFileSizeCacher.Set 2"))
                {
                    if (this._cacheDictionary.TryGetValue(newCache.FilePath, out var cache))
                    {
                        if (newCache.UpdateDate == cache.UpdateDate)
                        {
                            return;
                        }

                        this._cacheDictionary.Remove(cache.FilePath);
                    }

                    this._cacheDictionary.Add(newCache.FilePath, newCache);
                }
            }
        }

        public void Set(string filePath, Size size)
        {
            var updateDate = FileUtil.GetUpdateDate(filePath);
            this.Set(filePath, size, updateDate);
        }
    }
}
