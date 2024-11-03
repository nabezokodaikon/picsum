using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using PicSum.Job.Entities;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using System.Runtime.Versioning;

namespace PicSum.Job.Common
{
    /// <summary>
    /// サムネイルを読込みます。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed partial class ThumbnailCacher
        : IDisposable
    {
        private const int CACHE_CAPACITY = 1000;

        public readonly static ThumbnailCacher Instance = new();

        private bool disposed = false;
        private readonly int FILE_READ_BUFFER_SIZE = 1024 * 4;
        private readonly int BUFFER_FILE_MAX_SIZE = 1024 * 1024 * 10;
        private readonly List<ThumbnailCacheEntity> CACHE_LIST = new(CACHE_CAPACITY);
        private readonly Dictionary<string, ThumbnailCacheEntity> CACHE_DICTIONARY = new(CACHE_CAPACITY);
        private readonly SemaphoreSlim CACHE_LOCK = new(1, 1);

        private ThumbnailCacher()
        {

        }

        ~ThumbnailCacher()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.CACHE_LOCK.Dispose();
            }

            this.disposed = true;
        }

        internal ThumbnailCacheEntity GetOnlyCache(string filePath, int thumbWidth, int thumbHeight)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (FileUtil.IsSystemRoot(filePath))
            {
                return ThumbnailCacheEntity.EMPTY;
            }
            else if (FileUtil.IsDrive(filePath))
            {
                return ThumbnailCacheEntity.EMPTY;
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
                return ThumbnailCacheEntity.EMPTY;
            }
        }

        internal ThumbnailCacheEntity GetOrCreateCache(string filePath, int thumbWidth, int thumbHeight)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (FileUtil.IsSystemRoot(filePath))
            {
                return ThumbnailCacheEntity.EMPTY;
            }
            else if (FileUtil.IsDrive(filePath))
            {
                return ThumbnailCacheEntity.EMPTY;
            }
            else if (FileUtil.IsFile(filePath))
            {
                if (!FileUtil.IsImageFile(filePath))
                {
                    return ThumbnailCacheEntity.EMPTY;
                }

                var cache = this.GetOrCreateFileCache(filePath, thumbWidth, thumbHeight);
                if (cache != ThumbnailCacheEntity.EMPTY)
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
                            var thumb = new ThumbnailCacheEntity
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
                    return ThumbnailCacheEntity.EMPTY;
                }
            }
            else
            {
                var cache = this.GetOrCreateDirectoryCache(filePath, thumbWidth, thumbHeight);
                if (cache != ThumbnailCacheEntity.EMPTY)
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
                            var thumb = new ThumbnailCacheEntity
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
                    return ThumbnailCacheEntity.EMPTY;
                }
            }
        }

        private ThumbnailCacheEntity GetOnlyFileCache(string filePath, int thumbWidth, int thumbHeight)
        {
            var memCache = this.GetMemoryCache(filePath);
            if (memCache != ThumbnailCacheEntity.EMPTY)
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
                if (dbCache != ThumbnailCacheEntity.EMPTY)
                {
                    var updateDate = FileUtil.GetUpdateDate(filePath);
                    if (dbCache.ThumbnailWidth >= thumbWidth &&
                        dbCache.ThumbnailHeight >= thumbHeight &&
                        dbCache.FileUpdatedate >= updateDate)
                    {
                        // DBキャッシュを返します。
                        this.UpdateMemoryCache(dbCache);
                        return dbCache;
                    }
                }
            }

            return ThumbnailCacheEntity.EMPTY;
        }

        private ThumbnailCacheEntity GetOnlyDirectoryCache(string filePath, int thumbWidth, int thumbHeight)
        {
            var memCache = this.GetMemoryCache(filePath);
            if (memCache != ThumbnailCacheEntity.EMPTY)
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
                if (dbCache != ThumbnailCacheEntity.EMPTY)
                {
                    var updateDate = FileUtil.GetUpdateDate(filePath);
                    if (dbCache.ThumbnailWidth >= thumbWidth &&
                        dbCache.ThumbnailHeight >= thumbHeight &&
                        dbCache.FileUpdatedate >= updateDate)
                    {
                        // DBキャッシュを返します。
                        this.UpdateMemoryCache(dbCache);
                        return dbCache;
                    }
                }
            }

            return ThumbnailCacheEntity.EMPTY;
        }

        private ThumbnailCacheEntity GetOrCreateFileCache(string filePath, int thumbWidth, int thumbHeight)
        {
            var memCache = this.GetMemoryCache(filePath);
            if (memCache != ThumbnailCacheEntity.EMPTY)
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
                    if (dbCache != ThumbnailCacheEntity.EMPTY)
                    {
                        if (dbCache.ThumbnailWidth >= thumbWidth &&
                            dbCache.ThumbnailHeight >= thumbHeight &&
                            dbCache.FileUpdatedate >= updateDate)
                        {
                            // DBキャッシュを返します。
                            this.UpdateMemoryCache(dbCache);
                            return dbCache;
                        }
                        else
                        {
                            // サムネイルを更新します。
                            var thumb = this.UpdateDBFileCache(filePath, thumbWidth, thumbHeight, updateDate);
                            this.UpdateMemoryCache(thumb);
                            return thumb;
                        }
                    }
                    else
                    {
                        // サムネイルを作成します。
                        var thumb = this.CreateDBFileCache(filePath, thumbWidth, thumbHeight, updateDate);
                        this.UpdateMemoryCache(thumb);
                        return thumb;
                    }
                }
            }
            else
            {
                var dbCache = this.GetDBCache(filePath);
                if (dbCache != ThumbnailCacheEntity.EMPTY)
                {
                    var updateDate = FileUtil.GetUpdateDate(filePath);

                    if (dbCache.ThumbnailWidth >= thumbWidth &&
                        dbCache.ThumbnailHeight >= thumbHeight &&
                        dbCache.FileUpdatedate >= updateDate)
                    {
                        // DBキャッシュを返します。
                        this.UpdateMemoryCache(dbCache);
                        return dbCache;
                    }
                    else
                    {
                        // サムネイルを更新します。
                        var thumb = this.UpdateDBFileCache(filePath, thumbWidth, thumbHeight, updateDate);
                        this.UpdateMemoryCache(thumb);
                        return thumb;
                    }
                }
                else
                {
                    // サムネイルを作成します。
                    var updateDate = FileUtil.GetUpdateDate(filePath);
                    var thumb = this.CreateDBFileCache(filePath, thumbWidth, thumbHeight, updateDate);
                    this.UpdateMemoryCache(thumb);
                    return thumb;
                }
            }
        }

        private ThumbnailCacheEntity GetOrCreateDirectoryCache(string filePath, int thumbWidth, int thumbHeight)
        {
            var memCache = this.GetMemoryCache(filePath);
            if (memCache != ThumbnailCacheEntity.EMPTY)
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
                    if (dbCache != ThumbnailCacheEntity.EMPTY)
                    {
                        if (dbCache.ThumbnailWidth >= thumbWidth &&
                            dbCache.ThumbnailHeight >= thumbHeight &&
                            dbCache.FileUpdatedate >= updateDate)
                        {
                            // DBキャッシュを返します。
                            this.UpdateMemoryCache(dbCache);
                            return dbCache;
                        }
                        else
                        {
                            var thumbFile = FileUtil.GetFirstImageFilePath(filePath);
                            if (!string.IsNullOrEmpty(thumbFile))
                            {
                                // サムネイルを更新します。
                                var thumb = this.UpdateDBDirectoryCache(filePath, thumbFile, thumbWidth, thumbHeight, updateDate);
                                this.UpdateMemoryCache(thumb);
                                return thumb;
                            }
                            else
                            {
                                return ThumbnailCacheEntity.EMPTY;
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
                            this.UpdateMemoryCache(thumb);
                            return thumb;
                        }
                        else
                        {
                            return ThumbnailCacheEntity.EMPTY;
                        }
                    }
                }
            }
            else
            {
                var dbCache = this.GetDBCache(filePath);
                if (dbCache != ThumbnailCacheEntity.EMPTY)
                {
                    var updateDate = FileUtil.GetUpdateDate(filePath);

                    if (dbCache.ThumbnailWidth >= thumbWidth &&
                        dbCache.ThumbnailHeight >= thumbHeight &&
                        dbCache.FileUpdatedate >= updateDate)
                    {
                        // DBキャッシュを返します。
                        this.UpdateMemoryCache(dbCache);
                        return dbCache;
                    }
                    else
                    {
                        var thumbFile = FileUtil.GetFirstImageFilePath(filePath);
                        if (!string.IsNullOrEmpty(thumbFile))
                        {
                            // サムネイルを更新します。                                
                            var thumb = this.UpdateDBDirectoryCache(filePath, thumbFile, thumbWidth, thumbHeight, updateDate);
                            this.UpdateMemoryCache(thumb);
                            return thumb;
                        }
                        else
                        {
                            return ThumbnailCacheEntity.EMPTY;
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
                        this.UpdateMemoryCache(thumb);
                        return thumb;
                    }
                    else
                    {
                        return ThumbnailCacheEntity.EMPTY;
                    }
                }
            }
        }

        private string GetThumbnailBufferFilePath(int id)
        {
            return Path.Combine(
                ResourceUtil.DATABASE_DIRECTORY,
                $"{id}{ResourceUtil.THUMBNAIL_BUFFER_FILE_EXTENSION}");
        }

        private int GetCurrentThumbnailBufferID()
        {
            var id = (int)DatabaseManager<ThumbnailConnection>.ReadValue<long>(new ThumbnailIDReadSql());
            var thumbFile = this.GetThumbnailBufferFilePath(id);
            if (!File.Exists(thumbFile))
            {
                return id;
            }

            var size = FileUtil.GetFileSize(thumbFile);
            if (size < this.BUFFER_FILE_MAX_SIZE)
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
                filePath, FileMode.Open, FileAccess.Read, FileShare.Read, this.FILE_READ_BUFFER_SIZE, FileOptions.RandomAccess))
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
                thumbFile, FileMode.Append, FileAccess.Write, FileShare.None, this.FILE_READ_BUFFER_SIZE, FileOptions.None))
            using (var bs = new BufferedStream(fs, this.FILE_READ_BUFFER_SIZE))
            {
                var offset = (int)fs.Length;
                bs.Write(buffer, 0, buffer.Length);
                return offset;
            }
        }

        private ThumbnailCacheEntity GetDBCache(string filePath)
        {
            var sql = new ThumbnailReadByFileSql(filePath);
            var dto = DatabaseManager<ThumbnailConnection>.ReadLine(sql);
            if (!dto.Equals(default(ThumbnailDto)))
            {
                var thumb = new ThumbnailCacheEntity
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
                return ThumbnailCacheEntity.EMPTY;
            }
        }

        private ThumbnailCacheEntity CreateDBFileCache(
            string filePath, int thumbWidth, int thumbHeight, DateTime fileUpdateDate)
        {
            using (var srcImg = ImageUtil.ReadImageFile(filePath))
            {
                ImageFileSizeCacher.Set(filePath, srcImg.Size);
                using (var thumbImg = ThumbnailUtil.CreateThumbnail(srcImg, thumbWidth, thumbHeight))
                {
                    var thumbBin = ThumbnailUtil.ToCompressionBinary(thumbImg);
                    var thumbID = this.GetCurrentThumbnailBufferID();
                    var thumbStartPoint = this.AddThumbnailBuffer(thumbID, thumbBin);

                    var sql = new ThumbnailCreationSql(
                        filePath, thumbID, thumbStartPoint, thumbBin.Length, thumbWidth, thumbHeight, srcImg.Width, srcImg.Height, fileUpdateDate);
                    DatabaseManager<ThumbnailConnection>.Update(sql);

                    var thumb = new ThumbnailCacheEntity
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

        private ThumbnailCacheEntity UpdateDBFileCache(string filePath, int thumbWidth, int thumbHeight, DateTime fileUpdateDate)
        {
            using (var srcImg = ImageUtil.ReadImageFile(filePath))
            {
                ImageFileSizeCacher.Set(filePath, srcImg.Size);
                using (var thumbImg = ThumbnailUtil.CreateThumbnail(srcImg, thumbWidth, thumbHeight))
                {
                    var thumbBin = ThumbnailUtil.ToCompressionBinary(thumbImg);
                    var thumbID = this.GetCurrentThumbnailBufferID();
                    var thumbStartPoint = this.AddThumbnailBuffer(thumbID, thumbBin);

                    var sql = new ThumbnailUpdateSql(
                        filePath, thumbID, thumbStartPoint, thumbBin.Length, thumbWidth, thumbHeight, srcImg.Width, srcImg.Height, fileUpdateDate);
                    DatabaseManager<ThumbnailConnection>.Update(sql);

                    var thumb = new ThumbnailCacheEntity
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

        private ThumbnailCacheEntity CreateDBDirectoryCache(string directoryPath, string thumbFilePath, int thumbWidth, int thumbHeight, DateTime directoryUpdateDate)
        {
            using (var srcImg = ImageUtil.ReadImageFile(thumbFilePath))
            {
                ImageFileSizeCacher.Set(thumbFilePath, srcImg.Size);
                using (var thumbImg = ThumbnailUtil.CreateThumbnail(srcImg, thumbWidth, thumbHeight))
                {
                    var thumbBin = ThumbnailUtil.ToCompressionBinary(thumbImg);
                    var thumbID = this.GetCurrentThumbnailBufferID();
                    var thumbStartPoint = this.AddThumbnailBuffer(thumbID, thumbBin);

                    var sql = new ThumbnailCreationSql(
                        directoryPath, thumbID, thumbStartPoint, thumbBin.Length, thumbWidth, thumbHeight, srcImg.Width, srcImg.Height, directoryUpdateDate);
                    DatabaseManager<ThumbnailConnection>.Update(sql);

                    var thumb = new ThumbnailCacheEntity
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

        private ThumbnailCacheEntity UpdateDBDirectoryCache(string directoryPath, string thumbFilePath, int thumbWidth, int thumbHeight, DateTime directoryUpdateDate)
        {
            using (var srcImg = ImageUtil.ReadImageFile(thumbFilePath))
            {
                ImageFileSizeCacher.Set(thumbFilePath, srcImg.Size);
                using (var thumbImg = ThumbnailUtil.CreateThumbnail(srcImg, thumbWidth, thumbHeight))
                {
                    var thumbBin = ThumbnailUtil.ToCompressionBinary(thumbImg);
                    var thumbID = this.GetCurrentThumbnailBufferID();
                    var thumbStartPoint = this.AddThumbnailBuffer(thumbID, thumbBin);

                    var sql = new ThumbnailUpdateSql(
                        directoryPath, thumbID, thumbStartPoint, thumbBin.Length, thumbWidth, thumbHeight, srcImg.Width, srcImg.Height, directoryUpdateDate);
                    DatabaseManager<ThumbnailConnection>.Update(sql);

                    var thumb = new ThumbnailCacheEntity
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

        private ThumbnailCacheEntity GetMemoryCache(string filePath)
        {
            this.CACHE_LOCK.Wait();

            try
            {
                if (this.CACHE_DICTIONARY.TryGetValue(filePath, out var cach))
                {
                    return cach;
                }
                else
                {
                    return ThumbnailCacheEntity.EMPTY;
                }
            }
            finally
            {
                this.CACHE_LOCK.Release();
            }
        }

        private void UpdateMemoryCache(ThumbnailCacheEntity thumb)
        {
            if (thumb.FilePath == null)
            {
                throw new ArgumentException("サムネイルのファイルパスがNULLです。", nameof(thumb));
            }

            this.CACHE_LOCK.Wait();

            try
            {
                if (this.CACHE_DICTIONARY.TryGetValue(thumb.FilePath, out var dicCache))
                {
                    this.CACHE_LIST.Remove(dicCache);
                    this.CACHE_LIST.Add(thumb);
                    this.CACHE_DICTIONARY[thumb.FilePath] = thumb;
                }
                else
                {
                    if (this.CACHE_LIST.Count == this.CACHE_LIST.Capacity)
                    {
                        var Cache = this.CACHE_LIST[0];
                        this.CACHE_LIST.Remove(Cache);
                        this.CACHE_DICTIONARY.Remove(thumb.FilePath);
                    }

                    this.CACHE_LIST.Add(thumb);
                    this.CACHE_DICTIONARY.Add(thumb.FilePath, thumb);
                }
            }
            finally
            {
                this.CACHE_LOCK.Release();
            }
        }
    }
}
