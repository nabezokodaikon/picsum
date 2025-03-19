using NLog;
using OpenCvSharp.Extensions;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class ImageFileCacher
        : IImageFileCacher
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const int CACHE_CAPACITY = 16;

        private bool disposed = false;
        private readonly List<ImageFileCacheEntity> CACHE_LIST = new(CACHE_CAPACITY);
        private readonly Dictionary<string, ImageFileCacheEntity> CACHE_DICTIONARY = new(CACHE_CAPACITY);
        private readonly Lock CACHE_LOCK = new();

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
            }

            this.disposed = true;
        }

        public bool Has(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return this.Get(filePath, cache =>
            {
                if (cache != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        public Size GetSize(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var size = this.Get(filePath, cache =>
            {
                if (cache != null)
                {
                    return cache.Bitmap.Size;
                }
                else
                {
                    return ImageUtil.EMPTY_SIZE;
                }
            });

            if (size != ImageUtil.EMPTY_SIZE)
            {
                return size;
            }

            return ImageUtil.GetImageSize(filePath);
        }

        public CvImage GetCvImage(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var cvImage = this.Get(filePath, cache =>
            {
                if (cache != null)
                {
                    return new CvImage(cache.Bitmap.ToMat(), cache.Bitmap.PixelFormat);
                }
                else
                {
                    return null;
                }
            });

            if (cvImage != null)
            {
                return cvImage;
            }

            using (var bmp = ImageUtil.ReadImageFile(filePath))
            {
                return new CvImage(bmp.ToMat(), bmp.PixelFormat);
            }
        }

        public void Create(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            lock (this.CACHE_LOCK)
            {
                using (TimeMeasuring.Run(true, $"ImageFileCacher.Create 1"))
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

            var bitmap = ImageUtil.ReadImageFile(filePath);
            var newCache = new ImageFileCacheEntity(filePath, bitmap, timestamp);

            lock (this.CACHE_LOCK)
            {
                using (TimeMeasuring.Run(true, $"ImageFileCacher.Create 2"))
                {
                    if (this.CACHE_DICTIONARY.TryGetValue(newCache.FilePath, out var cache))
                    {
                        if (newCache.Timestamp == cache.Timestamp)
                        {
                            newCache.Dispose();
                            return;
                        }

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
                        Logger.Debug($"画像ファイルキャッシュ削除しました。: {removeCache.FilePath}");
                    }

                    this.CACHE_DICTIONARY.Add(newCache.FilePath, newCache);
                    this.CACHE_LIST.Add(newCache);
                    Logger.Debug($"画像ファイルをキャッシュしました。: {newCache.FilePath}");
                }
            }
        }

        private T Get<T>(string filePath, Func<ImageFileCacheEntity?, T> resultFunc)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            lock (this.CACHE_LOCK)
            {
                using (TimeMeasuring.Run(true, $"ImageFileCacher.Get {typeof(T)}"))
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
            }
        }
    }
}
