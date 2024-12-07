using OpenCvSharp.Extensions;
using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class ImageFileCacher
        : IImageFileCacher
    {
        private const int CACHE_CAPACITY = 12;

        private bool disposed = false;
        private readonly List<ImageFileCacheEntity> CACHE_LIST = new(CACHE_CAPACITY);
        private readonly Dictionary<string, ImageFileCacheEntity> CACHE_DICTIONARY = new(CACHE_CAPACITY);
        private readonly ReaderWriterLockSlim CACHE_LOCK = new();

        public ImageFileCacher()
        {

        }

        ~ImageFileCacher()
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
                foreach (var cache in this.CACHE_LIST)
                {
                    cache.Dispose();
                }

                this.CACHE_LOCK.Dispose();
            }

            this.disposed = true;
        }

        public Size GetSize(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return this.Get(filePath, cache =>
            {
                if (cache != null && cache.Bitmap != null)
                {
                    return cache.Bitmap.Size;
                }
                else
                {
                    return ImageUtil.GetImageSize(filePath);
                }
            });
        }

        public CvImage GetCvImage(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return this.Get(filePath, cache =>
            {
                if (cache != null && cache.Bitmap != null)
                {
                    return new CvImage(cache.Bitmap.ToMat(), cache.Bitmap.PixelFormat);
                }
                else
                {
                    using (var bmp = ImageUtil.ReadImageFile(filePath))
                    {
                        return new CvImage(bmp.ToMat(), bmp.PixelFormat);
                    }
                }
            });
        }

        public void Create(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);
            var bitmap = ImageUtil.ReadImageFile(filePath);
            var newCache = new ImageFileCacheEntity(filePath, bitmap, timestamp);

            this.CACHE_LOCK.EnterReadLock();
            try
            {
                if (this.CACHE_DICTIONARY.TryGetValue(newCache.FilePath, out var cache))
                {
                    if (newCache.Timestamp == cache.Timestamp)
                    {
                        newCache.Dispose();
                        return;
                    }
                }
            }
            finally
            {
                this.CACHE_LOCK.ExitReadLock();
            }

            this.CACHE_LOCK.EnterUpgradeableReadLock();
            try
            {
                if (this.CACHE_DICTIONARY.TryGetValue(newCache.FilePath, out var cache))
                {
                    if (newCache.Timestamp == cache.Timestamp)
                    {
                        newCache.Dispose();
                        return;
                    }
                }

                this.CACHE_LOCK.EnterWriteLock();
                try
                {
                    if (cache != null)
                    {
                        this.CACHE_LIST.Remove(cache);
                        this.CACHE_DICTIONARY.Remove(cache.FilePath);
                        cache.Dispose();
                    }

                    if (this.CACHE_LIST.Count > CACHE_CAPACITY)
                    {
                        var removeCache = this.CACHE_LIST[0];
                        this.CACHE_LIST.Remove(removeCache);
                        this.CACHE_DICTIONARY.Remove(removeCache.FilePath);
                        removeCache.Dispose();
                    }

                    this.CACHE_DICTIONARY.Add(newCache.FilePath, newCache);
                    this.CACHE_LIST.Add(newCache);
                }
                finally
                {
                    this.CACHE_LOCK.ExitWriteLock();
                }
            }
            finally
            {
                this.CACHE_LOCK.ExitUpgradeableReadLock();
            }
        }

        private T Get<T>(string filePath, Func<ImageFileCacheEntity?, T> resultFunc)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            this.CACHE_LOCK.EnterReadLock();
            try
            {
                if (this.CACHE_DICTIONARY.TryGetValue(filePath, out var cache))
                {
                    if (timestamp == cache.Timestamp)
                    {
                        return resultFunc(cache);
                    }
                }

                return resultFunc(null);
            }
            finally
            {
                this.CACHE_LOCK.ExitReadLock();
            }
        }
    }
}
