using NLog;
using OpenCvSharp.Extensions;
using SWF.Core.ConsoleAccessor;
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

        private bool _disposed = false;
        private readonly List<ImageFileCacheEntity> _cacheList = new(CACHE_CAPACITY);
        private readonly Dictionary<string, ImageFileCacheEntity> _cacheDictionary = new(CACHE_CAPACITY);
        private readonly Lock _cacheLock = new();

        public ImageFileCacher()
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
                foreach (var cache in this._cacheList)
                {
                    cache.Dispose();
                }
            }

            this._disposed = true;
        }

        public bool Has(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return this.Get(filePath, cache =>
            {
                if (cache != ImageFileCacheEntity.EMPTY)
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
                if (cache != ImageFileCacheEntity.EMPTY && cache.Bitmap != null)
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

        public CvImage GetCvImage(string filePath, float zoomValue)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var cvImage = this.Get(filePath, cache =>
            {
                if (cache != ImageFileCacheEntity.EMPTY && cache.Bitmap != null)
                {
                    return new CvImage(
                        filePath, cache.Bitmap.ToMat(), zoomValue);
                }
                else
                {
                    return CvImage.EMPTY;
                }
            });

            if (cvImage != CvImage.EMPTY)
            {
                return cvImage;
            }

            using (var bmp = ImageUtil.ReadImageFile(filePath))
            {
                return new CvImage(
                    filePath, bmp.ToMat(), zoomValue);
            }
        }

        public void Create(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            lock (this._cacheLock)
            {
                using (TimeMeasuring.Run(false, $"ImageFileCacher.Create 1"))
                {
                    if (this._cacheDictionary.TryGetValue(filePath, out var cache))
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

            lock (this._cacheLock)
            {
                using (TimeMeasuring.Run(false, $"ImageFileCacher.Create 2"))
                {
                    if (this._cacheDictionary.TryGetValue(newCache.FilePath, out var cache))
                    {
                        if (newCache.Timestamp == cache.Timestamp)
                        {
                            newCache.Dispose();
                            return;
                        }

                        this._cacheList.RemoveAll(_ => _.FilePath == cache.FilePath);
                        this._cacheDictionary.Remove(cache.FilePath);
                        cache.Dispose();
                    }

                    if (this._cacheList.Count > CACHE_CAPACITY)
                    {
                        var removeCache = this._cacheList[0];
                        this._cacheList.RemoveAt(0);
                        this._cacheDictionary.Remove(removeCache.FilePath);
                        removeCache.Dispose();
                        Logger.Debug($"画像ファイルキャッシュ削除しました。: {removeCache.FilePath}");
                    }

                    this._cacheDictionary.Add(newCache.FilePath, newCache);
                    this._cacheList.Add(newCache);
                    Logger.Debug($"画像ファイルをキャッシュしました。: {newCache.FilePath}");
                }
            }
        }

        private T Get<T>(string filePath, Func<ImageFileCacheEntity, T> resultFunc)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            lock (this._cacheLock)
            {
                using (TimeMeasuring.Run(false, $"ImageFileCacher.Get {typeof(T)}"))
                {
                    if (this._cacheDictionary.TryGetValue(filePath, out var cache))
                    {
                        if (timestamp == cache.Timestamp)
                        {
                            return resultFunc(cache);
                        }
                    }

                    return resultFunc(ImageFileCacheEntity.EMPTY);
                }
            }
        }
    }
}
