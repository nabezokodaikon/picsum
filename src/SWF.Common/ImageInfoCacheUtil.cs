using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Versioning;
using System.Threading;

namespace SWF.Common
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

        public static ImageInfoCache GetImageInfo(string filePath)
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
                        return cache;
                    }
                }

                CACHE_LOCK.EnterWriteLock();
                try
                {
                    if (CACHE_LIST.Count > CACHE_CAPACITY)
                    {
                        var removeCache = CACHE_LIST[0];
                        CACHE_LIST.Remove(removeCache);
                        CACHE_DICTIONARY.Remove(removeCache.FilePath);
                    }

                    var newItem = new ImageInfoCache(
                        filePath, ImageUtil.GetImageSize(filePath), false, timestamp);
                    CACHE_LIST.Add(newItem);
                    CACHE_DICTIONARY.Add(filePath, newItem);
                    return newItem;
                }
                finally
                {
                    CACHE_LOCK.ExitWriteLock();
                }
            }
            finally
            {
                CACHE_LOCK.ExitUpgradeableReadLock();
            }
        }

        public static void Create(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            CACHE_LOCK.EnterUpgradeableReadLock();
            try
            {
                if (CACHE_DICTIONARY.ContainsKey(filePath))
                {
                    return;
                }

                CACHE_LOCK.EnterWriteLock();
                try
                {
                    if (CACHE_LIST.Count > CACHE_CAPACITY)
                    {
                        var removeCache = CACHE_LIST[0];
                        CACHE_LIST.Remove(removeCache);
                        CACHE_DICTIONARY.Remove(removeCache.FilePath);
                    }

                    var newItem = new ImageInfoCache(
                        filePath, ImageUtil.GetImageSize(filePath), false, timestamp);
                    CACHE_LIST.Add(newItem);
                    CACHE_DICTIONARY.Add(filePath, newItem);
                }
                finally
                {
                    CACHE_LOCK.ExitWriteLock();
                }
            }
            finally
            {
                CACHE_LOCK.ExitUpgradeableReadLock();
            }
        }
    }
}
