namespace SWF.Core.ImageAccessor
{
    public class ImageFileReadedTimeCacheUtil
    {
        private const int CACHE_CAPACITY = 1000;
        private static readonly List<ImageFileReadedTimeCache> CACHE_LIST = new(CACHE_CAPACITY);
        private static readonly Dictionary<string, ImageFileReadedTimeCache> CACHE_DICTIONARY = new(CACHE_CAPACITY);
        private static readonly ReaderWriterLockSlim CACHE_LOCK = new();

        public static void DisposeStaticResouces()
        {
            CACHE_LOCK.Dispose();
            CACHE_LIST.Clear();
            CACHE_DICTIONARY.Clear();
        }

        public static bool IsSlow(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            CACHE_LOCK.EnterReadLock();
            try
            {
                if (CACHE_DICTIONARY.TryGetValue(filePath, out var cache))
                {
                    return cache.ReadedMilliseconds > 300;
                }
                else
                {
                    return true;
                }
            }
            finally
            {
                CACHE_LOCK.ExitReadLock();
            }
        }

        public static void Create(string filePath, long readedMilliseconds)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            CACHE_LOCK.EnterUpgradeableReadLock();
            try
            {
                ImageFileReadedTimeCache cache;
                if (CACHE_DICTIONARY.TryGetValue(filePath, out cache))
                {
                    if (readedMilliseconds > cache.ReadedMilliseconds)
                    {
                        return;
                    }
                }

                CACHE_LOCK.EnterWriteLock();
                try
                {
                    if (cache.FilePath != null)
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

                    var newCache = new ImageFileReadedTimeCache(
                        filePath, readedMilliseconds);
                    CACHE_LIST.Add(newCache);
                    CACHE_DICTIONARY.Add(filePath, newCache);
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
