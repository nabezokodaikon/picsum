using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows")]
    public static class ImageFileCacheUtil
    {
        private const int CACHE_CAPACITY = 15;
        private static readonly List<ImageFileCache> CACHE_LIST = new(CACHE_CAPACITY);
        private static readonly Dictionary<string, ImageFileCache> CACHE_DICTIONARY = new(CACHE_CAPACITY);
        private static readonly ReaderWriterLockSlim CACHE_LOCK = new();

        public static void DisposeStaticResouces()
        {
            CACHE_LOCK.Dispose();

            foreach (var entity in CACHE_LIST)
            {
                entity.Dispose();
            }

            CACHE_LIST.Clear();
            CACHE_DICTIONARY.Clear();
        }

        public static bool Contains(string filePath)
        {
            CACHE_LOCK.EnterReadLock();
            try
            {
                return CACHE_DICTIONARY.ContainsKey(filePath);
            }
            finally
            {
                CACHE_LOCK.ExitReadLock();
            }
        }

        public static Size GetSize(string filePath)
        {
            return Read(filePath, cache => cache.Image.Size);
        }

        public static Bitmap ReadImage(string filePath)
        {
            return Read(filePath, cache => cache.Clone()).Image;
        }

        private static T Read<T>(string filePath, Func<ImageFileCache, T> resultFunc)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var timestamp = FileUtil.GetUpdateDate(filePath);

            CACHE_LOCK.EnterUpgradeableReadLock();
            try
            {
                ImageFileCache cache = null;
                if (CACHE_DICTIONARY.TryGetValue(filePath, out cache))
                {
                    if (timestamp == cache.Timestamp)
                    {
                        return resultFunc(cache);
                    }
                }

                CACHE_LOCK.EnterWriteLock();
                try
                {
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
                        removeCache.Dispose();
                    }

                    var newCache = new ImageFileCache(
                        filePath, ImageUtil.ReadImageFileFast(filePath), timestamp);
                    CACHE_LIST.Add(newCache);
                    CACHE_DICTIONARY.Add(filePath, newCache);
                    return resultFunc(newCache);

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
