using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows")]
    public static class ImageFileSizeCacheUtil
    {
        private const int CACHE_CAPACITY = 1000;
        private static readonly List<ImageFileSizeCache> CACHE_LIST = new(CACHE_CAPACITY);
        private static readonly Dictionary<string, ImageFileSizeCache> CACHE_DICTIONARY = new(CACHE_CAPACITY);
        private static readonly ReaderWriterLockSlim CACHE_LOCK = new();

        public static void DisposeStaticResouces()
        {
            CACHE_LOCK.Dispose();
        }

        public static void Create(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            CACHE_LOCK.EnterWriteLock();
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
                }

                if (CACHE_LIST.Count > CACHE_CAPACITY)
                {
                    var removeCache = CACHE_LIST[0];
                    CACHE_LIST.Remove(removeCache);
                    CACHE_DICTIONARY.Remove(removeCache.FilePath);
                }

                var newCache = new ImageFileSizeCache(
                    filePath, ImageUtil.GetImageSize(filePath), timestamp);
                CACHE_DICTIONARY.Add(newCache.FilePath, newCache);
                CACHE_LIST.Add(newCache);
            }
            finally
            {
                CACHE_LOCK.ExitWriteLock();
            }
        }

        public static ImageFileSizeCache Get(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            CACHE_LOCK.EnterWriteLock();
            try
            {
                if (CACHE_DICTIONARY.TryGetValue(filePath, out var cache))
                {
                    if (timestamp == cache.Timestamp)
                    {
                        return cache;
                    }
                }

                if (cache != null)
                {
                    CACHE_LIST.Remove(cache);
                    CACHE_DICTIONARY.Remove(cache.FilePath);
                }

                if (CACHE_LIST.Count > CACHE_CAPACITY)
                {
                    var removeCache = CACHE_LIST[0];
                    CACHE_LIST.Remove(removeCache);
                    CACHE_DICTIONARY.Remove(removeCache.FilePath);
                }

                var newCache = new ImageFileSizeCache(
                    filePath, ImageUtil.GetImageSize(filePath), timestamp);
                CACHE_DICTIONARY.Add(newCache.FilePath, newCache);
                CACHE_LIST.Add(newCache);
                return newCache;
            }
            finally
            {
                CACHE_LOCK.ExitWriteLock();
            }
        }

        public static void Set(string filePath, Size size)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            CACHE_LOCK.EnterWriteLock();
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
                }

                if (CACHE_LIST.Count > CACHE_CAPACITY)
                {
                    var removeCache = CACHE_LIST[0];
                    CACHE_LIST.Remove(removeCache);
                    CACHE_DICTIONARY.Remove(removeCache.FilePath);
                }

                var newCache = new ImageFileSizeCache(
                    filePath, size, timestamp);
                CACHE_DICTIONARY.Add(newCache.FilePath, newCache);
                CACHE_LIST.Add(newCache);
            }
            finally
            {
                CACHE_LOCK.ExitWriteLock();
            }
        }
    }
}
