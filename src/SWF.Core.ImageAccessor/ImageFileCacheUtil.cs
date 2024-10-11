using SWF.Core.FileAccessor;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows")]
    public static class ImageFileCacheUtil
    {
        private const int CACHE_CAPACITY = 16;
        private static readonly List<ImageFileCache> CACHE_LIST = new(CACHE_CAPACITY);
        private static readonly Dictionary<string, ImageFileCache> CACHE_DICTIONARY = new(CACHE_CAPACITY);
        private static readonly ReaderWriterLockSlim CACHE_LOCK = new();

        public static void DisposeStaticResouces()
        {

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
            return Read(filePath, cache => cache.Buffer.Size);
        }

        public static Bitmap GetBitmap(string filePath)
        {
            return Read(filePath, cache => cache.Buffer.ToBitmap());
        }

        public static CvImage GetCvImage(string filePath)
        {
            return Read(filePath, cache => new CvImage(cache.Buffer.ToBitmap()));
        }

        public static void Create(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var sw = Stopwatch.StartNew();

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
            finally
            {
                CACHE_LOCK.ExitUpgradeableReadLock();

                sw.Stop();
                //Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageFileCacheUtil.Create: {sw.ElapsedMilliseconds} ms");
            }
        }

        private static T Read<T>(string filePath, Func<ImageFileCache, T> resultFunc)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var sw = Stopwatch.StartNew();

            var timestamp = FileUtil.GetUpdateDate(filePath);

            CACHE_LOCK.EnterUpgradeableReadLock();
            try
            {
                if (CACHE_DICTIONARY.TryGetValue(filePath, out var cache))
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
            finally
            {
                CACHE_LOCK.ExitUpgradeableReadLock();

                sw.Stop();
                //Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageFileCacheUtil.Read: {sw.ElapsedMilliseconds} ms");
            }
        }
    }
}
