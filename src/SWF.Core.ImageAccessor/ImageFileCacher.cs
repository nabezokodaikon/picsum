using SWF.Core.Base;
using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class ImageFileCacher
        : IImageFileCacher
    {
        private const int CACHE_CAPACITY = 10;

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

        public Size GetSize(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return this.Get(filePath, static cache =>
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
        }

        public CvImage GetCache(string filePath, float zoomValue)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return this.Get(filePath, cache =>
            {
                if (cache != ImageFileCacheEntity.EMPTY && cache.Bitmap != null)
                {
                    return new CvImage(
                        filePath, OpenCVUtil.ToMat(cache.Bitmap), zoomValue);
                }
                else
                {
                    return CvImage.EMPTY;
                }
            });
        }

        public void Create(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var updateDate = FileUtil.GetUpdateDate(filePath);

            lock (this._cacheLock)
            {
                if (this._cacheDictionary.TryGetValue(filePath, out var cache))
                {
                    if (updateDate == cache.UpdateDate)
                    {
                        return;
                    }
                }
            }

            var bitmap = ImageUtil.ReadImageFile(filePath);
            var newCache = new ImageFileCacheEntity(filePath, bitmap, updateDate);

            lock (this._cacheLock)
            {
                if (this._cacheDictionary.TryGetValue(filePath, out var cache))
                {
                    if (updateDate == cache.UpdateDate)
                    {
                        newCache.Dispose();
                        return;
                    }
                }

                if (this._cacheList.Count > CACHE_CAPACITY)
                {
                    var removeCache = this._cacheList[0];
                    this._cacheList.RemoveAt(0);
                    this._cacheDictionary.Remove(removeCache.FilePath);
                    removeCache.Dispose();
                }

                this._cacheDictionary.Add(newCache.FilePath, newCache);
                this._cacheList.Add(newCache);
            }
        }

        private T Get<T>(string filePath, Func<ImageFileCacheEntity, T> resultFunc)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            using (TimeMeasuring.Run(false, $"ImageFileCacher.Get {typeof(T)}"))
            {
                var updateDate = FileUtil.GetUpdateDate(filePath);

                lock (this._cacheLock)
                {
                    if (this._cacheDictionary.TryGetValue(filePath, out var cache))
                    {
                        if (updateDate == cache.UpdateDate)
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
