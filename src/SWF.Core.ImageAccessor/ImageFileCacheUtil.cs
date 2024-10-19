using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows")]
    public static class ImageFileCacheUtil
    {
        private const int CACHE_CAPACITY = 8;
        private static readonly List<ImageFileCache> CACHE_LIST = new(CACHE_CAPACITY);
        private static readonly Dictionary<string, ImageFileCache> CACHE_DICTIONARY = new(CACHE_CAPACITY);
        private static readonly ReaderWriterLockSlim CACHE_LOCK = new();

        public static void DisposeStaticResouces()
        {
            CACHE_LOCK.Dispose();
        }

        public static bool HasCache(string filePath)
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
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return Read(filePath, static cache =>
            {
                if (cache.Buffer == null)
                {
                    throw new NullReferenceException("キャッシュのバッファがNullです。");
                }

                return cache.Buffer.Size;
            });
        }

        public static CvImage GetCvImage(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return Read(filePath, static cache =>
            {
                if (cache.Buffer == null)
                {
                    throw new NullReferenceException("キャッシュのバッファがNullです。");
                }

                return new CvImage(cache.Buffer.ToBitmap());
            });
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
                    cache.Dispose();
                }

                if (CACHE_LIST.Count > CACHE_CAPACITY)
                {
                    var removeCache = CACHE_LIST[0];
                    CACHE_LIST.Remove(removeCache);
                    CACHE_DICTIONARY.Remove(removeCache.FilePath);
                    removeCache.Dispose();
                }

                var buffer = ImageUtil.ReadImageFileBuffer(filePath);
                ImageFileSizeCacheUtil.Set(filePath, buffer.Size);
                var newCache = new ImageFileCache(filePath, buffer, timestamp);
                CACHE_DICTIONARY.Add(newCache.FilePath, newCache);
                CACHE_LIST.Add(newCache);
            }
            finally
            {
                CACHE_LOCK.ExitWriteLock();
            }
        }

        private static T Read<T>(string filePath, Func<ImageFileCache, T> resultFunc)
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

                var buffer = ImageUtil.ReadImageFileBuffer(filePath);
                ImageFileSizeCacheUtil.Set(filePath, buffer.Size);
                var newCache = new ImageFileCache(filePath, buffer, timestamp);
                CACHE_DICTIONARY.Add(filePath, newCache);
                CACHE_LIST.Add(newCache);
                return resultFunc(newCache);
            }
            finally
            {
                CACHE_LOCK.ExitWriteLock();
            }
        }
    }
}
