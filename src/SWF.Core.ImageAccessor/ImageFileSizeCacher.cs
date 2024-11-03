using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows")]
    public sealed partial class ImageFileSizeCacher
        : IDisposable
    {
        private const int CACHE_CAPACITY = 1000;

        public readonly static ImageFileSizeCacher Instance = new();

        private bool disposed = false;
        private readonly List<ImageFileSizeCacheEntity> CACHE_LIST = new(CACHE_CAPACITY);
        private readonly Dictionary<string, ImageFileSizeCacheEntity> CACHE_DICTIONARY = new(CACHE_CAPACITY);
        private readonly object CACHE_LOCK = new();

        private ImageFileSizeCacher()
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
                if (this.CACHE_DICTIONARY.TryGetValue(filePath, out var cache))
                {
                    if (timestamp == cache.Timestamp)
                    {
                        return;
                    }
                }

                if (cache != null)
                {
                    this.CACHE_LIST.Remove(cache);
                    this.CACHE_DICTIONARY.Remove(cache.FilePath);
                }

                if (this.CACHE_LIST.Count > CACHE_CAPACITY)
                {
                    var removeCache = this.CACHE_LIST[0];
                    this.CACHE_LIST.Remove(removeCache);
                    this.CACHE_DICTIONARY.Remove(removeCache.FilePath);
                }

                var newCache = new ImageFileSizeCacheEntity(
                    filePath, ImageUtil.GetImageSize(filePath), timestamp);
                this.CACHE_DICTIONARY.Add(newCache.FilePath, newCache);
                this.CACHE_LIST.Add(newCache);
            }
        }

        public ImageFileSizeCacheEntity Get(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            lock (this.CACHE_LOCK)
            {
                if (this.CACHE_DICTIONARY.TryGetValue(filePath, out var cache))
                {
                    if (timestamp == cache.Timestamp)
                    {
                        return cache;
                    }
                }

                if (cache != null)
                {
                    this.CACHE_LIST.Remove(cache);
                    this.CACHE_DICTIONARY.Remove(cache.FilePath);
                }

                if (this.CACHE_LIST.Count > CACHE_CAPACITY)
                {
                    var removeCache = this.CACHE_LIST[0];
                    this.CACHE_LIST.Remove(removeCache);
                    this.CACHE_DICTIONARY.Remove(removeCache.FilePath);
                }

                var newCache = new ImageFileSizeCacheEntity(
                    filePath, ImageUtil.GetImageSize(filePath), timestamp);
                this.CACHE_DICTIONARY.Add(newCache.FilePath, newCache);
                this.CACHE_LIST.Add(newCache);
                return newCache;
            }
        }

        public void Set(string filePath, Size size)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            lock (this.CACHE_LOCK)
            {
                if (this.CACHE_DICTIONARY.TryGetValue(filePath, out var cache))
                {
                    if (timestamp == cache.Timestamp)
                    {
                        return;
                    }
                }

                if (cache != null)
                {
                    this.CACHE_LIST.Remove(cache);
                    this.CACHE_DICTIONARY.Remove(cache.FilePath);
                }

                if (this.CACHE_LIST.Count > CACHE_CAPACITY)
                {
                    var removeCache = this.CACHE_LIST[0];
                    this.CACHE_LIST.Remove(removeCache);
                    this.CACHE_DICTIONARY.Remove(removeCache.FilePath);
                }

                var newCache = new ImageFileSizeCacheEntity(
                    filePath, size, timestamp);
                this.CACHE_DICTIONARY.Add(newCache.FilePath, newCache);
                this.CACHE_LIST.Add(newCache);
            }
        }
    }
}
