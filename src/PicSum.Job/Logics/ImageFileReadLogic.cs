using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Entities;
using SWF.Common;
using System.Drawing;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// 画像ファイル読込ロジック
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class ImageFileReadLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        private const int CACHE_CAPACITY = 10;
        private static readonly List<ImageCacheEntity> CACHE_LIST = new(CACHE_CAPACITY);
        private static readonly Dictionary<string, ImageCacheEntity> CACHE_DICTIONARY = new(CACHE_CAPACITY);
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

        public Bitmap Execute(string filePath)
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
                        var clone = cache.Clone();
                        if (clone.Image == null)
                        {
                            throw new NullReferenceException("キャッシュの複製に画像が設定されていません。");
                        }

                        return clone.Image;
                    }
                }

                CACHE_LOCK.EnterWriteLock();
                try
                {
                    if (CACHE_LIST.Count > CACHE_CAPACITY)
                    {
                        var removeCache = CACHE_LIST[0];
                        CACHE_LIST.Remove(removeCache);
                        if (removeCache.FilePath == null)
                        {
                            throw new NullReferenceException("キャッシュにファイルパスが設定されていません。");
                        }

                        CACHE_DICTIONARY.Remove(removeCache.FilePath);
                        removeCache.Dispose();
                    }

                    var newImage = new ImageCacheEntity(
                        filePath, ImageUtil.ReadImageFile(filePath), timestamp);
                    CACHE_LIST.Add(newImage);
                    CACHE_DICTIONARY.Add(filePath, newImage);
                    var clone = newImage.Clone();
                    if (clone.Image == null)
                    {
                        throw new NullReferenceException("キャッシュの複製に画像が設定されていません。");
                    }
                    return clone.Image;
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

        public void Create(string filePath)
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
                        if (removeCache.FilePath == null)
                        {
                            throw new NullReferenceException("キャッシュにファイルパスが設定されていません。");
                        }

                        CACHE_DICTIONARY.Remove(removeCache.FilePath);
                        removeCache.Dispose();
                    }

                    var newImage = new ImageCacheEntity(
                        filePath, ImageUtil.ReadImageFile(filePath), timestamp);
                    CACHE_LIST.Add(newImage);
                    CACHE_DICTIONARY.Add(filePath, newImage);
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
