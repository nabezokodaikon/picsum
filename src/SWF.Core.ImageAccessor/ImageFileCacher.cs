using SWF.Core.Base;
using SWF.Core.FileAccessor;

namespace SWF.Core.ImageAccessor
{

    public sealed partial class ImageFileCacher
        : IImageFileCacher
    {
        private const int CACHE_CAPACITY = 4;

        private bool _disposed = false;
        private readonly LinkedList<ImageFileCacheEntity> _cacheList = new();
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
                if (!cache.IsEmpty && cache.Bitmap != null)
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
                if (!cache.IsEmpty && cache.Bitmap != null)
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

        public async ValueTask Create(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var updateDate = FileUtil.GetUpdateDate(filePath);

            lock (this._cacheLock)
            {
                using (TimeMeasuring.Run(false, $"ImageFileCacher.Create 1"))
                {
                    if (this._cacheDictionary.TryGetValue(filePath, out var cache))
                    {
                        if (updateDate == cache.UpdateDate)
                        {
                            if (this._cacheList.Remove(cache))
                            {
                                this._cacheList.AddFirst(cache);
                            }

                            return;
                        }
                    }
                }
            }

            var bitmap = await ImageUtil.ReadImageFile(filePath).WithConfig();
            var newCache = new ImageFileCacheEntity(filePath, bitmap, updateDate);

            lock (this._cacheLock)
            {
                using (TimeMeasuring.Run(false, $"ImageFileCacher.Create 2"))
                {
                    if (this._cacheDictionary.TryGetValue(filePath, out var cache))
                    {
                        if (updateDate == cache.UpdateDate)
                        {
                            newCache.Dispose();

                            if (this._cacheList.Remove(cache))
                            {
                                this._cacheList.AddFirst(cache);
                            }

                            return;
                        }
                    }

                    if (this._cacheList.Count > CACHE_CAPACITY)
                    {
                        var removeCache = this._cacheList.Last;
                        if (removeCache != null)
                        {
                            this._cacheList.RemoveLast();
                            this._cacheDictionary.Remove(removeCache.Value.FilePath);
                            removeCache.Value.Dispose();
                        }
                    }

                    this._cacheDictionary.Add(newCache.FilePath, newCache);
                    this._cacheList.AddFirst(newCache);
                }
            }
        }

        private T Get<T>(string filePath, Func<ImageFileCacheEntity, T> resultFunc)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var updateDate = FileUtil.GetUpdateDate(filePath);

            lock (this._cacheLock)
            {
                using (TimeMeasuring.Run(false, $"ImageFileCacher.Get {typeof(T)}"))
                {
                    if (this._cacheDictionary.TryGetValue(filePath, out var cache))
                    {
                        if (updateDate == cache.UpdateDate)
                        {
                            if (this._cacheList.Remove(cache))
                            {
                                this._cacheList.AddFirst(cache);
                            }

                            return resultFunc(cache);
                        }
                    }

                    return resultFunc(ImageFileCacheEntity.EMPTY);
                }
            }
        }
    }
}
