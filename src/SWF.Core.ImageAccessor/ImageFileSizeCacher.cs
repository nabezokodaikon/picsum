using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows")]
    public static class ImageFileSizeCacher
    {
        private const int CACHE_CAPACITY = 1000;
        private static readonly List<ImageFileSizeCacheEntity> CACHE_LIST = new(CACHE_CAPACITY);
        private static readonly Dictionary<string, ImageFileSizeCacheEntity> CACHE_DICTIONARY = new(CACHE_CAPACITY);
        private static readonly object CACHE_LOCK = new();

        public static void Create(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            lock (CACHE_LOCK)
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

                var newCache = new ImageFileSizeCacheEntity(
                    filePath, ImageUtil.GetImageSize(filePath), timestamp);
                CACHE_DICTIONARY.Add(newCache.FilePath, newCache);
                CACHE_LIST.Add(newCache);
            }
        }

        public static ImageFileSizeCacheEntity Get(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            lock (CACHE_LOCK)
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

                var newCache = new ImageFileSizeCacheEntity(
                    filePath, ImageUtil.GetImageSize(filePath), timestamp);
                CACHE_DICTIONARY.Add(newCache.FilePath, newCache);
                CACHE_LIST.Add(newCache);
                return newCache;
            }
        }

        public static void Set(string filePath, Size size)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            lock (CACHE_LOCK)
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

                var newCache = new ImageFileSizeCacheEntity(
                    filePath, size, timestamp);
                CACHE_DICTIONARY.Add(newCache.FilePath, newCache);
                CACHE_LIST.Add(newCache);
            }
        }
    }
}
