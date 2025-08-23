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
        private readonly SemaphoreSlim _cacheLock = new(1, 1);

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
                this._cacheLock.Dispose();
            }

            this._disposed = true;
        }

        public async ValueTask Create(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var updateDate = FileUtil.GetUpdateDate(filePath);

            await this._cacheLock.WaitAsync().WithConfig();
            try
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
            finally
            {
                this._cacheLock.Release();
            }

            var newCache = new ImageFileSizeCacheEntity(
                filePath, await ImageUtil.GetImageSize(filePath).WithConfig(), updateDate);

            await this._cacheLock.WaitAsync().WithConfig();
            try
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
            finally
            {
                this._cacheLock.Release();
            }
        }

        public async ValueTask<ImageFileSizeCacheEntity> GetOrCreate(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            using (TimeMeasuring.Run(false, "ImageFileSizeCacher.GetOrCreate"))
            {
                var updateDate = FileUtil.GetUpdateDate(filePath);

                await this._cacheLock.WaitAsync().WithConfig();
                try
                {
                    if (this._cacheDictionary.TryGetValue(filePath, out var cache))
                    {
                        if (updateDate == cache.UpdateDate)
                        {
                            return cache;
                        }
                    }
                }
                finally
                {
                    this._cacheLock.Release();
                }

                var size = await ImageUtil.GetImageSize(filePath).WithConfig();
                await this.Set(filePath, size, updateDate);
                return new ImageFileSizeCacheEntity(
                    filePath, size, updateDate);
            }
        }

        public async ValueTask Set(string filePath, Size size, DateTime updateDate)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            await this._cacheLock.WaitAsync().WithConfig();
            try
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
            finally
            {
                this._cacheLock.Release();
            }

            var newCache = new ImageFileSizeCacheEntity(
                filePath, size, updateDate);

            await this._cacheLock.WaitAsync().WithConfig();
            try
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
            finally
            {
                this._cacheLock.Release();
            }
        }

        public async ValueTask Set(string filePath, Size size)
        {
            var updateDate = FileUtil.GetUpdateDate(filePath);
            await this.Set(filePath, size, updateDate).WithConfig();
        }
    }
}
