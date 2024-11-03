using OpenCvSharp.Extensions;
using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows")]
    public sealed partial class ImageFileCacher
        : IDisposable
    {
        private const int CACHE_CAPACITY = 12;

        public readonly static ImageFileCacher Instance = new();

        private bool disposed = false;
        private readonly List<ImageFileCacheEntity> CACHE_LIST = new(CACHE_CAPACITY);
        private readonly Dictionary<string, ImageFileCacheEntity> CACHE_DICTIONARY = new(CACHE_CAPACITY);
        private readonly object CACHE_LOCK = new();

        private ImageFileCacher()
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
            }

            this.disposed = true;
        }

        public Size GetSize(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return this.Read(filePath, cache =>
            {
                if (cache.Bitmap == null)
                {
                    throw new NullReferenceException("キャッシュのBitmapがNullです。");
                }

                return cache.Bitmap.Size;
            });
        }

        public CvImage GetCvImage(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return this.Read(filePath, cache =>
            {
                if (cache.Bitmap == null)
                {
                    throw new NullReferenceException("キャッシュのBitmapがNullです。");
                }

                return new CvImage(cache.Bitmap.ToMat(), cache.Bitmap.PixelFormat);
            });
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
                    cache.Dispose();
                }

                if (this.CACHE_LIST.Count > CACHE_CAPACITY)
                {
                    var removeCache = this.CACHE_LIST[0];
                    this.CACHE_LIST.Remove(removeCache);
                    this.CACHE_DICTIONARY.Remove(removeCache.FilePath);
                    removeCache.Dispose();
                }

                var bitmap = ImageUtil.ReadImageFile(filePath);
                ImageFileSizeCacher.Set(filePath, bitmap.Size);
                var newCache = new ImageFileCacheEntity(filePath, bitmap, timestamp);
                this.CACHE_DICTIONARY.Add(newCache.FilePath, newCache);
                this.CACHE_LIST.Add(newCache);
            }
        }

        private T Read<T>(string filePath, Func<ImageFileCacheEntity, T> resultFunc)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            lock (this.CACHE_LOCK)
            {
                if (this.CACHE_DICTIONARY.TryGetValue(filePath, out var cache))
                {
                    if (timestamp == cache.Timestamp)
                    {
                        return resultFunc(cache);
                    }
                }

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

                var bitmap = ImageUtil.ReadImageFile(filePath);
                ImageFileSizeCacher.Set(filePath, bitmap.Size);
                var newCache = new ImageFileCacheEntity(filePath, bitmap, timestamp);
                this.CACHE_DICTIONARY.Add(filePath, newCache);
                this.CACHE_LIST.Add(newCache);
                return resultFunc(newCache);
            }
        }
    }
}
