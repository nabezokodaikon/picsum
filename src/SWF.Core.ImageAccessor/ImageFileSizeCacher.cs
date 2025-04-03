using SWF.Core.Base;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class ImageFileSizeCacher
        : IImageFileSizeCacher
    {
        private const int CACHE_CAPACITY = 1000;

        private bool disposed = false;
        private readonly List<ImageFileSizeCacheEntity> CACHE_LIST = new(CACHE_CAPACITY);
        private readonly Dictionary<string, ImageFileSizeCacheEntity> CACHE_DICTIONARY = new(CACHE_CAPACITY);
        private readonly Lock CACHE_LOCK = new();

        public ImageFileSizeCacher()
        {

        }

        ~ImageFileSizeCacher()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {

            }

            this.disposed = true;
        }

        public void Create(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            lock (this.CACHE_LOCK)
            {
                using (TimeMeasuring.Run(false, $"ImageFileSizeCacher.Create 1"))
                {
                    if (this.CACHE_DICTIONARY.TryGetValue(filePath, out var cache))
                    {
                        if (timestamp == cache.Timestamp)
                        {
                            return;
                        }
                    }
                }
            }

            var newCache = new ImageFileSizeCacheEntity(
                filePath, ImageUtil.GetImageSize(filePath), timestamp);

            lock (this.CACHE_LOCK)
            {
                using (TimeMeasuring.Run(false, $"ImageFileSizeCacher.Create 2"))
                {
                    if (this.CACHE_DICTIONARY.TryGetValue(filePath, out var cache))
                    {
                        if (newCache.Timestamp == cache.Timestamp)
                        {
                            return;
                        }

                        this.CACHE_LIST.RemoveAll(_ => _.FilePath == cache.FilePath);
                        this.CACHE_DICTIONARY.Remove(cache.FilePath);
                    }

                    if (this.CACHE_LIST.Count > CACHE_CAPACITY)
                    {
                        var removeCache = this.CACHE_LIST[0];
                        this.CACHE_LIST.RemoveAt(0);
                        this.CACHE_DICTIONARY.Remove(removeCache.FilePath);
                    }

                    this.CACHE_DICTIONARY.Add(newCache.FilePath, newCache);
                    this.CACHE_LIST.Add(newCache);
                }
            }
        }

        public ImageFileSizeCacheEntity Get(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            lock (this.CACHE_LOCK)
            {
                using (TimeMeasuring.Run(false, $"ImageFileSizeCacher.Get"))
                {
                    if (this.CACHE_DICTIONARY.TryGetValue(filePath, out var cache))
                    {
                        if (timestamp == cache.Timestamp)
                        {
                            return cache;
                        }
                    }
                }
            }

            return new ImageFileSizeCacheEntity(
                filePath, ImageUtil.GetImageSize(filePath), timestamp);
        }

        public void Set(string filePath, Size size)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            lock (this.CACHE_LOCK)
            {
                using (TimeMeasuring.Run(false, $"ImageFileSizeCacher.Set 1"))
                {
                    if (this.CACHE_DICTIONARY.TryGetValue(filePath, out var cache))
                    {
                        if (timestamp == cache.Timestamp)
                        {
                            return;
                        }
                    }
                }
            }

            var newCache = new ImageFileSizeCacheEntity(
                filePath, size, timestamp);

            lock (this.CACHE_LOCK)
            {
                using (TimeMeasuring.Run(false, $"ImageFileSizeCacher.Set 2"))
                {
                    if (this.CACHE_DICTIONARY.TryGetValue(newCache.FilePath, out var cache))
                    {
                        if (newCache.Timestamp == cache.Timestamp)
                        {
                            return;
                        }

                        this.CACHE_LIST.RemoveAll(_ => _.FilePath == cache.FilePath);
                        this.CACHE_DICTIONARY.Remove(cache.FilePath);
                    }

                    if (this.CACHE_LIST.Count > CACHE_CAPACITY)
                    {
                        var removeCache = this.CACHE_LIST[0];
                        this.CACHE_LIST.RemoveAt(0);
                        this.CACHE_DICTIONARY.Remove(removeCache.FilePath);
                    }

                    this.CACHE_DICTIONARY.Add(newCache.FilePath, newCache);
                    this.CACHE_LIST.Add(newCache);
                }
            }
        }
    }
}
