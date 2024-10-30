using OpenCvSharp.Extensions;
using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows")]
    public static class ImageFileCacheUtil
    {
        private const int CACHE_CAPACITY = 12;
        private static readonly List<ImageFileCache> CACHE_LIST = new(CACHE_CAPACITY);
        private static readonly Dictionary<string, ImageFileCache> CACHE_DICTIONARY = new(CACHE_CAPACITY);
        private static readonly SemaphoreSlim CACHE_LOCK = new(1, 1);

        public static void DisposeStaticResources()
        {
            CACHE_LOCK.Dispose();
        }

        public static Size GetSize(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return Read(filePath, static cache =>
            {
                if (cache.Bitmap == null)
                {
                    throw new NullReferenceException("キャッシュのBitmapがNullです。");
                }

                return cache.Bitmap.Size;
            });
        }

        public static CvImage GetCvImage(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return Read(filePath, static cache =>
            {
                if (cache.Bitmap == null)
                {
                    throw new NullReferenceException("キャッシュのBitmapがNullです。");
                }

                return new CvImage(cache.Bitmap.ToMat(), cache.Bitmap.PixelFormat);
            });
        }

        public static void Create(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            CACHE_LOCK.Wait();
            try
            {
                if (CACHE_DICTIONARY.TryGetValue(filePath, out var cache))
                {
                    if (timestamp == cache.Timestamp)
                    {
                        return;
                    }
                }

                if (cache != null)
                {
                    CACHE_LIST.Remove(cache);
                    CACHE_DICTIONARY.Remove(cache.FilePath);
                    cache.Dispose();
                }

                if (CACHE_LIST.Count > CACHE_CAPACITY)
                {
                    var removeCache = CACHE_LIST[0];
                    CACHE_LIST.Remove(removeCache);
                    CACHE_DICTIONARY.Remove(removeCache.FilePath);
                    removeCache.Dispose();
                }

                var bitmap = ImageUtil.ReadImageFile(filePath);
                ImageFileSizeCacheUtil.Set(filePath, bitmap.Size);
                var newCache = new ImageFileCache(filePath, bitmap, timestamp);
                CACHE_DICTIONARY.Add(newCache.FilePath, newCache);
                CACHE_LIST.Add(newCache);
            }
            finally
            {
                CACHE_LOCK.Release();
            }
        }

        private static T Read<T>(string filePath, Func<ImageFileCache, T> resultFunc)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            CACHE_LOCK.Wait();
            try
            {
                if (CACHE_DICTIONARY.TryGetValue(filePath, out var cache))
                {
                    if (timestamp == cache.Timestamp)
                    {
                        return resultFunc(cache);
                    }
                }

                if (cache != null)
                {
                    CACHE_LIST.Remove(cache);
                    CACHE_DICTIONARY.Remove(cache.FilePath);
                    cache.Dispose();
                }

                if (CACHE_LIST.Count > CACHE_CAPACITY)
                {
                    var removeCache = CACHE_LIST[0];
                    CACHE_LIST.Remove(removeCache);
                    CACHE_DICTIONARY.Remove(removeCache.FilePath);
                    removeCache.Dispose();
                }

                var bitmap = ImageUtil.ReadImageFile(filePath);
                ImageFileSizeCacheUtil.Set(filePath, bitmap.Size);
                var newCache = new ImageFileCache(filePath, bitmap, timestamp);
                CACHE_DICTIONARY.Add(filePath, newCache);
                CACHE_LIST.Add(newCache);
                return resultFunc(newCache);
            }
            finally
            {
                CACHE_LOCK.Release();
            }
        }
    }
}
