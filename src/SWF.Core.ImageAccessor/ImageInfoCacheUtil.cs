using SWF.Core.FileAccessor;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows")]
    public static class ImageInfoCacheUtil
    {
        private const int CACHE_CAPACITY = 1000;
        private static readonly List<ImageInfoCache> CACHE_LIST = new(CACHE_CAPACITY);
        private static readonly Dictionary<string, ImageInfoCache> CACHE_DICTIONARY = new(CACHE_CAPACITY);
        private static readonly ReaderWriterLockSlim CACHE_LOCK = new();

        public static void DisposeStaticResouces()
        {
            CACHE_LOCK.Dispose();
            CACHE_LIST.Clear();
            CACHE_DICTIONARY.Clear();
        }

        internal static ImageInfoCache GetImageInfo(string filePath)
        {
            var sw = Stopwatch.StartNew();

            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            CACHE_LOCK.EnterUpgradeableReadLock();
            try
            {
                if (CACHE_DICTIONARY.TryGetValue(filePath, out var cache))
                {
                    if (timestamp == cache.Timestamp)
                    {
                        return cache;
                    }

                    CACHE_LOCK.EnterWriteLock();
                    try
                    {
                        CACHE_LIST.Remove(cache);
                        CACHE_DICTIONARY.Remove(cache.FilePath);

                        var newCache = new ImageInfoCache(
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
                else
                {
                    CACHE_LOCK.EnterWriteLock();
                    try
                    {
                        if (CACHE_LIST.Count > CACHE_CAPACITY)
                        {
                            var removeCache = CACHE_LIST[0];
                            CACHE_LIST.Remove(removeCache);
                            CACHE_DICTIONARY.Remove(removeCache.FilePath);
                        }

                        var newCache = new ImageInfoCache(
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
            }
            finally
            {
                CACHE_LOCK.ExitUpgradeableReadLock();

                sw.Stop();
                Console.WriteLine($"ImageInfoCacheUtil.GetImageInfo: {sw.ElapsedMilliseconds} ms");
            }
        }

        public static void SetImageInfo(string filePath, Size size)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            CACHE_LOCK.EnterUpgradeableReadLock();
            try
            {
                if (CACHE_DICTIONARY.TryGetValue(filePath, out var cache))
                {
                    if (timestamp == cache.Timestamp)
                    {
                        return;
                    }

                    CACHE_LOCK.EnterWriteLock();
                    try
                    {
                        CACHE_LIST.Remove(cache);
                        CACHE_DICTIONARY.Remove(cache.FilePath);

                        var newCache = new ImageInfoCache(
                            filePath, size, timestamp);
                        CACHE_DICTIONARY.Add(newCache.FilePath, newCache);
                        CACHE_LIST.Add(newCache);
                    }
                    finally
                    {
                        CACHE_LOCK.ExitWriteLock();
                    }
                }
                else
                {
                    CACHE_LOCK.EnterWriteLock();
                    try
                    {
                        if (CACHE_LIST.Count > CACHE_CAPACITY)
                        {
                            var removeCache = CACHE_LIST[0];
                            CACHE_LIST.Remove(removeCache);
                            CACHE_DICTIONARY.Remove(removeCache.FilePath);
                        }

                        var newCache = new ImageInfoCache(
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
            finally
            {
                CACHE_LOCK.ExitUpgradeableReadLock();
            }
        }
    }
}
