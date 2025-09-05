using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;

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

        public async ValueTask Create(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var updateDate = FileUtil.GetUpdateDate(filePath);

            this._cacheLock.Enter();
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
                this._cacheLock.Exit();
            }

            var newCache = new ImageFileSizeCacheEntity(
                filePath, await ImageUtil.GetImageSize(filePath).False(), updateDate);

            this._cacheLock.Enter();
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
                this._cacheLock.Exit();
            }
        }

        public async ValueTask<ImageFileSizeCacheEntity> GetOrCreate(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            using (TimeMeasuring.Run(false, "ImageFileSizeCacher.GetOrCreate"))
            {
                var updateDate = FileUtil.GetUpdateDate(filePath);

                this._cacheLock.Enter();
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
                    this._cacheLock.Exit();
                }

                var size = await ImageUtil.GetImageSize(filePath).False();
                this.Set(filePath, size, updateDate);
                return new ImageFileSizeCacheEntity(
                    filePath, size, updateDate);
            }
        }

        public void Set(string filePath, Size size, DateTime updateDate)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this._cacheLock.Enter();
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
                this._cacheLock.Exit();
            }

            var newCache = new ImageFileSizeCacheEntity(
                filePath, size, updateDate);

            this._cacheLock.Enter();
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
                this._cacheLock.Exit();
            }
        }

        public void Set(string filePath, Size size)
        {
            var updateDate = FileUtil.GetUpdateDate(filePath);
            this.Set(filePath, size, updateDate);
        }
    }
}
