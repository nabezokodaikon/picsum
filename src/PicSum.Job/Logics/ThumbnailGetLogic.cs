using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using PicSum.Job.Entities;
using SWF.Core.DatabaseAccessor;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// サムネイルを読込みます。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class ThumbnailGetLogic
        : AbstractAsyncLogic
    {
        private const int CACHE_CAPACITY = 1000;
        private static readonly int FILE_READ_BUFFER_SIZE = 1024 * 4;
        private static readonly int BUFFER_FILE_MAX_SIZE = 1024 * 1024 * 10;
        private static readonly List<ThumbnailBufferEntity> CACHE_LIST = new(CACHE_CAPACITY);
        private static readonly Dictionary<string, ThumbnailBufferEntity> CACHE_DICTIONARY = new(CACHE_CAPACITY);
        private static readonly ReaderWriterLockSlim CACHE_LOCK = new();

        /// <summary>
        /// 静的リソースを解放します。
        /// </summary>
        public static void DisposeStaticResouces()
        {
            CACHE_LOCK.Dispose();
        }

        public ThumbnailGetLogic(AbstractAsyncJob job)
            : base(job)
        {

        }

        public ThumbnailBufferEntity GetOnlyCache(string filePath, int thumbWidth, int thumbHeight)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (FileUtil.IsSystemRoot(filePath))
            {
                return ThumbnailBufferEntity.EMPTY;
            }
            else if (FileUtil.IsDrive(filePath))
            {
                return ThumbnailBufferEntity.EMPTY;
            }
            else if (FileUtil.IsFile(filePath) && FileUtil.IsImageFile(filePath))
            {
                return this.GetOnlyFileCache(filePath, thumbWidth, thumbHeight);
            }
            else if (FileUtil.IsDirectory(filePath))
            {
                return this.GetOnlyDirectoryCache(filePath, thumbWidth, thumbHeight);
            }
            else
            {
                return ThumbnailBufferEntity.EMPTY;
            }
        }

        public ThumbnailBufferEntity GetOrCreateCache(string filePath, int thumbWidth, int thumbHeight)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (FileUtil.IsSystemRoot(filePath))
            {
                return ThumbnailBufferEntity.EMPTY;
            }
            else if (FileUtil.IsDrive(filePath))
            {
                return ThumbnailBufferEntity.EMPTY;
            }
            else if (FileUtil.IsFile(filePath))
            {
                if (!FileUtil.IsImageFile(filePath))
                {
                    return ThumbnailBufferEntity.EMPTY;
                }

                var cache = this.GetOrCreateFileCache(filePath, thumbWidth, thumbHeight);
                if (cache != ThumbnailBufferEntity.EMPTY)
                {
                    if (cache.ThumbnailWidth > thumbWidth || cache.ThumbnailHeight > thumbHeight)
                    {
                        if (cache.ThumbnailBuffer == null)
                        {
                            throw new NullReferenceException("サムネイルのバッファがNullです。");
                        }

                        using (var CacheThumb = ThumbnailUtil.ToImage(cache.ThumbnailBuffer))
                        using (var newThumb = ThumbnailUtil.CreateThumbnail(CacheThumb, thumbWidth, thumbHeight))
                        {
                            var thumb = new ThumbnailBufferEntity
                            {
                                FilePath = cache.FilePath,
                                ThumbnailBuffer = ThumbnailUtil.ToCompressionBinary(newThumb),
                                ThumbnailWidth = thumbWidth,
                                ThumbnailHeight = thumbHeight,
                                SourceWidth = cache.SourceWidth,
                                SourceHeight = cache.SourceHeight,
                                FileUpdatedate = cache.FileUpdatedate
                            };
                            return thumb;
                        }
                    }
                    else
                    {
                        return cache;
                    }
                }
                else
                {
                    return ThumbnailBufferEntity.EMPTY;
                }
            }
            else
            {
                var cache = this.GetOrCreateDirectoryCache(filePath, thumbWidth, thumbHeight);
                if (cache != ThumbnailBufferEntity.EMPTY)
                {
                    if (cache.ThumbnailWidth > thumbWidth || cache.ThumbnailHeight > thumbHeight)
                    {
                        if (cache.ThumbnailBuffer == null)
                        {
                            throw new NullReferenceException("サムネイルのバッファがNullです。");
                        }

                        using (var CacheThumb = ThumbnailUtil.ToImage(cache.ThumbnailBuffer))
                        using (var newThumb = ThumbnailUtil.CreateThumbnail(CacheThumb, thumbWidth, thumbHeight))
                        {
                            var thumb = new ThumbnailBufferEntity
                            {
                                FilePath = cache.FilePath,
                                ThumbnailBuffer = ThumbnailUtil.ToCompressionBinary(newThumb),
                                ThumbnailWidth = thumbWidth,
                                ThumbnailHeight = thumbHeight,
                                SourceWidth = cache.SourceWidth,
                                SourceHeight = cache.SourceHeight,
                                FileUpdatedate = cache.FileUpdatedate
                            };
                            return thumb;
                        }
                    }
                    else
                    {
                        return cache;
                    }
                }
                else
                {
                    return ThumbnailBufferEntity.EMPTY;
                }
            }
        }

        private ThumbnailBufferEntity GetOnlyFileCache(string filePath, int thumbWidth, int thumbHeight)
        {
            var memCache = GetMemoryCache(filePath);
            if (memCache != ThumbnailBufferEntity.EMPTY)
            {
                var updateDate = FileUtil.GetUpdateDate(filePath);
                if (memCache.ThumbnailWidth >= thumbWidth &&
                    memCache.ThumbnailHeight >= thumbHeight &&
                    memCache.FileUpdatedate >= updateDate)
                {
                    // メモリキャッシュを返します。
                    return memCache;
                }
            }
            else
            {
                var dbCache = this.GetDBCache(filePath);
                if (dbCache != ThumbnailBufferEntity.EMPTY)
                {
                    var updateDate = FileUtil.GetUpdateDate(filePath);
                    if (dbCache.ThumbnailWidth >= thumbWidth &&
                        dbCache.ThumbnailHeight >= thumbHeight &&
                        dbCache.FileUpdatedate >= updateDate)
                    {
                        // DBキャッシュを返します。
                        UpdateMemoryCache(dbCache);
                        return dbCache;
                    }
                }
            }

            return ThumbnailBufferEntity.EMPTY;
        }

        private ThumbnailBufferEntity GetOnlyDirectoryCache(string filePath, int thumbWidth, int thumbHeight)
        {
            var memCache = GetMemoryCache(filePath);
            if (memCache != ThumbnailBufferEntity.EMPTY)
            {
                var updateDate = FileUtil.GetUpdateDate(filePath);
                if (memCache.ThumbnailWidth >= thumbWidth &&
                    memCache.ThumbnailHeight >= thumbHeight &&
                    memCache.FileUpdatedate >= updateDate)
                {
                    // メモリキャッシュを返します。
                    return memCache;
                }
            }
            else
            {
                var dbCache = this.GetDBCache(filePath);
                if (dbCache != ThumbnailBufferEntity.EMPTY)
                {
                    var updateDate = FileUtil.GetUpdateDate(filePath);
                    if (dbCache.ThumbnailWidth >= thumbWidth &&
                        dbCache.ThumbnailHeight >= thumbHeight &&
                        dbCache.FileUpdatedate >= updateDate)
                    {
                        // DBキャッシュを返します。
                        UpdateMemoryCache(dbCache);
                        return dbCache;
                    }
                }
            }

            return ThumbnailBufferEntity.EMPTY;
        }

        private ThumbnailBufferEntity GetOrCreateFileCache(string filePath, int thumbWidth, int thumbHeight)
        {
            var memCache = GetMemoryCache(filePath);
            if (memCache != ThumbnailBufferEntity.EMPTY)
            {
                var updateDate = FileUtil.GetUpdateDate(filePath);
                if (memCache.ThumbnailWidth >= thumbWidth &&
                    memCache.ThumbnailHeight >= thumbHeight &&
                    memCache.FileUpdatedate >= updateDate)
                {
                    // メモリキャッシュを返します。
                    return memCache;
                }
                else
                {
                    var dbCache = this.GetDBCache(filePath);
                    if (dbCache != ThumbnailBufferEntity.EMPTY)
                    {
                        if (dbCache.ThumbnailWidth >= thumbWidth &&
                            dbCache.ThumbnailHeight >= thumbHeight &&
                            dbCache.FileUpdatedate >= updateDate)
                        {
                            // DBキャッシュを返します。
                            UpdateMemoryCache(dbCache);
                            return dbCache;
                        }
                        else
                        {
                            // サムネイルを更新します。
                            var thumb = this.UpdateDBFileCache(filePath, thumbWidth, thumbHeight, updateDate);
                            UpdateMemoryCache(thumb);
                            return thumb;
                        }
                    }
                    else
                    {
                        // サムネイルを作成します。
                        var thumb = this.CreateDBFileCache(filePath, thumbWidth, thumbHeight, updateDate);
                        UpdateMemoryCache(thumb);
                        return thumb;
                    }
                }
            }
            else
            {
                var dbCache = this.GetDBCache(filePath);
                if (dbCache != ThumbnailBufferEntity.EMPTY)
                {
                    var updateDate = FileUtil.GetUpdateDate(filePath);

                    if (dbCache.ThumbnailWidth >= thumbWidth &&
                        dbCache.ThumbnailHeight >= thumbHeight &&
                        dbCache.FileUpdatedate >= updateDate)
                    {
                        // DBキャッシュを返します。
                        UpdateMemoryCache(dbCache);
                        return dbCache;
                    }
                    else
                    {
                        // サムネイルを更新します。
                        var thumb = this.UpdateDBFileCache(filePath, thumbWidth, thumbHeight, updateDate);
                        UpdateMemoryCache(thumb);
                        return thumb;
                    }
                }
                else
                {
                    // サムネイルを作成します。
                    var updateDate = FileUtil.GetUpdateDate(filePath);
                    var thumb = this.CreateDBFileCache(filePath, thumbWidth, thumbHeight, updateDate);
                    UpdateMemoryCache(thumb);
                    return thumb;
                }
            }
        }

        private ThumbnailBufferEntity GetOrCreateDirectoryCache(string filePath, int thumbWidth, int thumbHeight)
        {
            var memCache = GetMemoryCache(filePath);
            if (memCache != ThumbnailBufferEntity.EMPTY)
            {
                var updateDate = FileUtil.GetUpdateDate(filePath);

                if (memCache.ThumbnailWidth >= thumbWidth &&
                    memCache.ThumbnailHeight >= thumbHeight &&
                    memCache.FileUpdatedate >= updateDate)
                {
                    // メモリキャッシュを返します。
                    return memCache;
                }
                else
                {
                    var dbCache = this.GetDBCache(filePath);
                    if (dbCache != ThumbnailBufferEntity.EMPTY)
                    {
                        if (dbCache.ThumbnailWidth >= thumbWidth &&
                            dbCache.ThumbnailHeight >= thumbHeight &&
                            dbCache.FileUpdatedate >= updateDate)
                        {
                            // DBキャッシュを返します。
                            UpdateMemoryCache(dbCache);
                            return dbCache;
                        }
                        else
                        {
                            var thumbFile = FileUtil.GetFirstImageFilePath(filePath);
                            if (!string.IsNullOrEmpty(thumbFile))
                            {
                                // サムネイルを更新します。
                                var thumb = this.UpdateDBDirectoryCache(filePath, thumbFile, thumbWidth, thumbHeight, updateDate);
                                UpdateMemoryCache(thumb);
                                return thumb;
                            }
                            else
                            {
                                return ThumbnailBufferEntity.EMPTY;
                            }
                        }
                    }
                    else
                    {
                        var thumbFile = FileUtil.GetFirstImageFilePath(filePath);
                        if (!string.IsNullOrEmpty(thumbFile))
                        {
                            // サムネイルを作成します。
                            var thumb = this.CreateDBDirectoryCache(filePath, thumbFile, thumbWidth, thumbHeight, updateDate);
                            UpdateMemoryCache(thumb);
                            return thumb;
                        }
                        else
                        {
                            return ThumbnailBufferEntity.EMPTY;
                        }
                    }
                }
            }
            else
            {
                var dbCache = this.GetDBCache(filePath);
                if (dbCache != ThumbnailBufferEntity.EMPTY)
                {
                    var updateDate = FileUtil.GetUpdateDate(filePath);

                    if (dbCache.ThumbnailWidth >= thumbWidth &&
                        dbCache.ThumbnailHeight >= thumbHeight &&
                        dbCache.FileUpdatedate >= updateDate)
                    {
                        // DBキャッシュを返します。
                        UpdateMemoryCache(dbCache);
                        return dbCache;
                    }
                    else
                    {
                        var thumbFile = FileUtil.GetFirstImageFilePath(filePath);
                        if (!string.IsNullOrEmpty(thumbFile))
                        {
                            // サムネイルを更新します。                                
                            var thumb = this.UpdateDBDirectoryCache(filePath, thumbFile, thumbWidth, thumbHeight, updateDate);
                            UpdateMemoryCache(thumb);
                            return thumb;
                        }
                        else
                        {
                            return ThumbnailBufferEntity.EMPTY;
                        }
                    }
                }
                else
                {
                    // サムネイルを作成します。
                    var thumbFile = FileUtil.GetFirstImageFilePath(filePath);
                    if (!string.IsNullOrEmpty(thumbFile))
                    {
                        var updateDate = FileUtil.GetUpdateDate(filePath);
                        var thumb = this.CreateDBDirectoryCache(filePath, thumbFile, thumbWidth, thumbHeight, updateDate);
                        UpdateMemoryCache(thumb);
                        return thumb;
                    }
                    else
                    {
                        return ThumbnailBufferEntity.EMPTY;
                    }
                }
            }
        }

        private string GetThumbnailBufferFilePath(int id)
        {
            var dbDir = FileUtil.GetParentDirectoryPath(DatabaseManager<ThumbnailConnection>.DBFilePath);
            return Path.Combine(dbDir, $"{id}.thumbnail");
        }

        private int GetCurrentThumbnailBufferID()
        {
            var id = (int)DatabaseManager<ThumbnailConnection>.ReadValue<long>(new ThumbnailIDReadSql());
            var thumbFile = this.GetThumbnailBufferFilePath(id);
            if (!File.Exists(thumbFile))
            {
                return id;
            }

            var fi = new FileInfo(thumbFile);
            var size = fi.Length;
            if (size < BUFFER_FILE_MAX_SIZE)
            {
                return id;
            }
            else
            {
                DatabaseManager<ThumbnailConnection>.Update(new ThumbnailIDUpdateSql());
                var newID = (int)DatabaseManager<ThumbnailConnection>.ReadValue<long>(new ThumbnailIDReadSql());
                return newID;
            }
        }

        private byte[] ReadThumbnailBuffer(string filePath, int startPoint, int size)
        {
            using (var fs = new FileStream(
                filePath, FileMode.Open, FileAccess.Read, FileShare.Read, FILE_READ_BUFFER_SIZE, FileOptions.RandomAccess))
            {
                var bf = new byte[size];
                fs.Seek(startPoint, SeekOrigin.Begin);
                fs.Read(bf, 0, size);
                return bf;
            }
        }

        private int AddThumbnailBuffer(int id, byte[] buffer)
        {
            var thumbFile = this.GetThumbnailBufferFilePath(id);
            using (var fs = new FileStream(
                thumbFile, FileMode.Append, FileAccess.Write, FileShare.None, FILE_READ_BUFFER_SIZE, FileOptions.None))
            using (var bs = new BufferedStream(fs, FILE_READ_BUFFER_SIZE))
            {
                var offset = (int)fs.Length;
                bs.Write(buffer, 0, buffer.Length);
                return offset;
            }
        }

        private ThumbnailBufferEntity GetDBCache(string filePath)
        {
            var sql = new ThumbnailReadByFileSql(filePath);
            var dto = DatabaseManager<ThumbnailConnection>.ReadLine<ThumbnailDto>(sql);
            if (!dto.Equals(default(ThumbnailDto)))
            {
                var thumb = new ThumbnailBufferEntity
                {
                    FilePath = dto.FilePath,
                    ThumbnailWidth = dto.ThumbnailWidth,
                    ThumbnailHeight = dto.ThumbnailHeight,
                    SourceWidth = dto.SourceWidth,
                    SourceHeight = dto.SourceHeight,
                    FileUpdatedate = dto.FileUpdatedate
                };

                var thumbBufferFile = this.GetThumbnailBufferFilePath(dto.ThumbnailID);
                thumb.ThumbnailBuffer = this.ReadThumbnailBuffer(thumbBufferFile, dto.ThumbnailStartPoint, dto.ThumbnailSize);

                return thumb;
            }
            else
            {
                return ThumbnailBufferEntity.EMPTY;
            }
        }

        private ThumbnailBufferEntity CreateDBFileCache(
            string filePath, int thumbWidth, int thumbHeight, DateTime fileUpdateDate)
        {
            using (var srcImg = ImageUtil.ReadImageFile(filePath))
            {
                ImageFileSizeCacheUtil.Set(filePath, srcImg.Size);
                using (var thumbImg = ThumbnailUtil.CreateThumbnail(srcImg, thumbWidth, thumbHeight))
                {
                    var thumbBin = ThumbnailUtil.ToCompressionBinary(thumbImg);
                    var thumbID = this.GetCurrentThumbnailBufferID();
                    var thumbStartPoint = this.AddThumbnailBuffer(thumbID, thumbBin);

                    var sql = new ThumbnailCreationSql(
                        filePath, thumbID, thumbStartPoint, thumbBin.Length, thumbWidth, thumbHeight, srcImg.Width, srcImg.Height, fileUpdateDate);
                    DatabaseManager<ThumbnailConnection>.Update(sql);

                    var thumb = new ThumbnailBufferEntity
                    {
                        FilePath = filePath,
                        ThumbnailBuffer = thumbBin,
                        ThumbnailWidth = thumbWidth,
                        ThumbnailHeight = thumbHeight,
                        SourceWidth = srcImg.Width,
                        SourceHeight = srcImg.Height,
                        FileUpdatedate = fileUpdateDate
                    };

                    return thumb;
                }
            }
        }

        private ThumbnailBufferEntity UpdateDBFileCache(string filePath, int thumbWidth, int thumbHeight, DateTime fileUpdateDate)
        {
            using (var srcImg = ImageUtil.ReadImageFile(filePath))
            {
                ImageFileSizeCacheUtil.Set(filePath, srcImg.Size);
                using (var thumbImg = ThumbnailUtil.CreateThumbnail(srcImg, thumbWidth, thumbHeight))
                {
                    var thumbBin = ThumbnailUtil.ToCompressionBinary(thumbImg);
                    var thumbID = this.GetCurrentThumbnailBufferID();
                    var thumbStartPoint = this.AddThumbnailBuffer(thumbID, thumbBin);

                    var sql = new ThumbnailUpdateSql(
                        filePath, thumbID, thumbStartPoint, thumbBin.Length, thumbWidth, thumbHeight, srcImg.Width, srcImg.Height, fileUpdateDate);
                    DatabaseManager<ThumbnailConnection>.Update(sql);

                    var thumb = new ThumbnailBufferEntity
                    {
                        FilePath = filePath,
                        ThumbnailBuffer = thumbBin,
                        ThumbnailWidth = thumbWidth,
                        ThumbnailHeight = thumbHeight,
                        SourceWidth = srcImg.Width,
                        SourceHeight = srcImg.Height,
                        FileUpdatedate = fileUpdateDate
                    };

                    return thumb;
                }
            }
        }

        private ThumbnailBufferEntity CreateDBDirectoryCache(string directoryPath, string thumbFilePath, int thumbWidth, int thumbHeight, DateTime directoryUpdateDate)
        {
            using (var srcImg = ImageUtil.ReadImageFile(thumbFilePath))
            {
                ImageFileSizeCacheUtil.Set(thumbFilePath, srcImg.Size);
                using (var thumbImg = ThumbnailUtil.CreateThumbnail(srcImg, thumbWidth, thumbHeight))
                {
                    var thumbBin = ThumbnailUtil.ToCompressionBinary(thumbImg);
                    var thumbID = this.GetCurrentThumbnailBufferID();
                    var thumbStartPoint = this.AddThumbnailBuffer(thumbID, thumbBin);

                    var sql = new ThumbnailCreationSql(
                        directoryPath, thumbID, thumbStartPoint, thumbBin.Length, thumbWidth, thumbHeight, srcImg.Width, srcImg.Height, directoryUpdateDate);
                    DatabaseManager<ThumbnailConnection>.Update(sql);

                    var thumb = new ThumbnailBufferEntity
                    {
                        FilePath = directoryPath,
                        ThumbnailBuffer = thumbBin,
                        ThumbnailWidth = thumbWidth,
                        ThumbnailHeight = thumbHeight,
                        SourceWidth = srcImg.Width,
                        SourceHeight = srcImg.Height,
                        FileUpdatedate = directoryUpdateDate
                    };
                    return thumb;
                }
            }
        }

        private ThumbnailBufferEntity UpdateDBDirectoryCache(string directoryPath, string thumbFilePath, int thumbWidth, int thumbHeight, DateTime directoryUpdateDate)
        {
            using (var srcImg = ImageUtil.ReadImageFile(thumbFilePath))
            {
                ImageFileSizeCacheUtil.Set(thumbFilePath, srcImg.Size);
                using (var thumbImg = ThumbnailUtil.CreateThumbnail(srcImg, thumbWidth, thumbHeight))
                {
                    var thumbBin = ThumbnailUtil.ToCompressionBinary(thumbImg);
                    var thumbID = this.GetCurrentThumbnailBufferID();
                    var thumbStartPoint = this.AddThumbnailBuffer(thumbID, thumbBin);

                    var sql = new ThumbnailUpdateSql(
                        directoryPath, thumbID, thumbStartPoint, thumbBin.Length, thumbWidth, thumbHeight, srcImg.Width, srcImg.Height, directoryUpdateDate);
                    DatabaseManager<ThumbnailConnection>.Update(sql);

                    var thumb = new ThumbnailBufferEntity
                    {
                        FilePath = directoryPath,
                        ThumbnailBuffer = thumbBin,
                        ThumbnailWidth = thumbWidth,
                        ThumbnailHeight = thumbHeight,
                        SourceWidth = srcImg.Width,
                        SourceHeight = srcImg.Height,
                        FileUpdatedate = directoryUpdateDate
                    };

                    return thumb;
                }
            }
        }

        private static ThumbnailBufferEntity GetMemoryCache(string filePath)
        {
            CACHE_LOCK.EnterWriteLock();

            try
            {
                if (CACHE_DICTIONARY.TryGetValue(filePath, out var cach))
                {
                    return cach;
                }
                else
                {
                    return ThumbnailBufferEntity.EMPTY;
                }
            }
            finally
            {
                CACHE_LOCK.ExitWriteLock();
            }
        }

        private static void UpdateMemoryCache(ThumbnailBufferEntity thumb)
        {
            if (thumb.FilePath == null)
            {
                throw new ArgumentException("サムネイルのファイルパスがNULLです。", nameof(thumb));
            }

            CACHE_LOCK.EnterWriteLock();

            try
            {
                if (CACHE_DICTIONARY.TryGetValue(thumb.FilePath, out var dicCache))
                {
                    CACHE_LIST.Remove(dicCache);
                    CACHE_LIST.Add(thumb);
                    CACHE_DICTIONARY[thumb.FilePath] = thumb;
                }
                else
                {
                    if (CACHE_LIST.Count == CACHE_LIST.Capacity)
                    {
                        var Cache = CACHE_LIST[0];
                        CACHE_LIST.Remove(Cache);
                        CACHE_DICTIONARY.Remove(thumb.FilePath);
                    }

                    CACHE_LIST.Add(thumb);
                    CACHE_DICTIONARY.Add(thumb.FilePath, thumb);
                }
            }
            finally
            {
                CACHE_LOCK.ExitWriteLock();
            }
        }
    }
}
