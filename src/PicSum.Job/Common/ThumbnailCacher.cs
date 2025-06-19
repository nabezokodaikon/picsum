using PicSum.DatabaseAccessor.Connection;
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
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed partial class ThumbnailCacher
        : IThumbnailCacher
    {
        private const int CACHE_CAPACITY = 1000;
        private const int BUFFER_FILE_MAX_SIZE = 1024 * 1024 * 10;

        private static string GetThumbnailBufferFilePath(int id)
        {
            return Path.Combine(
                AppConstants.DATABASE_DIRECTORY.Value,
                $"{id}{ThumbnailUtil.THUMBNAIL_BUFFER_FILE_EXTENSION}");
        }

        private static int GetCurrentThumbnailBufferID(IDBConnection con)
        {
            var id = (int)con.ReadValue<long>(new ThumbnailIDReadSql());
            var thumbFile = GetThumbnailBufferFilePath(id);
            if (!FileUtil.IsExistsFile(thumbFile))
            {
                return id;
            }

            var size = FileUtil.GetFileSize(thumbFile);
            if (size < BUFFER_FILE_MAX_SIZE)
            {
                return id;
            }

            con.Update(new ThumbnailIDUpdateSql());
            var newID = (int)con.ReadValue<long>(new ThumbnailIDReadSql());
            return newID;
        }

        private bool _disposed = false;
        private readonly List<ThumbnailCacheEntity> _cacheList = new(CACHE_CAPACITY);
        private readonly Dictionary<string, ThumbnailCacheEntity> _cacheDictionary = new(CACHE_CAPACITY);
        private readonly FileAppender _fileAppender = new();
        private readonly Lock _cacheLock = new();

        public ThumbnailCacher()
        {

        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {

            }

            this._disposed = true;
        }

        public ThumbnailCacheEntity GetCache(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (FileUtil.IsExistsFile(filePath) && ImageUtil.IsImageFile(filePath))
            {
                return this.GetFileCache(filePath);
            }
            else if (FileUtil.IsExistsDirectory(filePath))
            {
                return this.GetDirectoryCache(filePath);
            }
            else if (FileUtil.IsSystemRoot(filePath))
            {
                return ThumbnailCacheEntity.EMPTY;
            }
            else if (FileUtil.IsExistsDrive(filePath))
            {
                return ThumbnailCacheEntity.EMPTY;
            }
            else
            {
                return ThumbnailCacheEntity.EMPTY;
            }
        }

        public ThumbnailCacheEntity GetOrCreateCache(
            string filePath, int thumbWidth, int thumbHeight)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (FileUtil.IsExistsFile(filePath))
            {
                if (!ImageUtil.IsImageFile(filePath))
                {
                    return ThumbnailCacheEntity.EMPTY;
                }

                var cache = this.GetOrCreateFileCache(filePath, thumbWidth, thumbHeight);
                if (cache != ThumbnailCacheEntity.EMPTY)
                {
                    return cache;
                }
                else
                {
                    return ThumbnailCacheEntity.EMPTY;
                }
            }
            else if (FileUtil.IsExistsDirectory(filePath))
            {
                var cache = this.GetOrCreateDirectoryCache(filePath, thumbWidth, thumbHeight);
                if (cache != ThumbnailCacheEntity.EMPTY)
                {
                    return cache;
                }
                else
                {
                    return ThumbnailCacheEntity.EMPTY;
                }
            }
            else if (FileUtil.IsSystemRoot(filePath))
            {
                return ThumbnailCacheEntity.EMPTY;
            }
            else if (FileUtil.IsExistsDrive(filePath))
            {
                return ThumbnailCacheEntity.EMPTY;
            }
            else
            {
                return ThumbnailCacheEntity.EMPTY;
            }
        }

        private ThumbnailCacheEntity GetFileCache(string filePath)
        {
            var memCache = this.GetMemoryCache(filePath);
            if (memCache != ThumbnailCacheEntity.EMPTY)
            {
                var updateDate = FileUtil.GetUpdateDate(filePath);
                if (memCache.FileUpdatedate >= updateDate)
                {
                    // メモリキャッシュを返します。
                    return memCache;
                }
            }

            return ThumbnailCacheEntity.EMPTY;
        }

        private ThumbnailCacheEntity GetDirectoryCache(string directoryPath)
        {
            return this.GetFileCache(directoryPath);
        }

        private ThumbnailCacheEntity GetOrCreateFileCache(
            string filePath, int thumbWidth, int thumbHeight)
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
                            var thumb = this.UpdateDBFileCache(
                                filePath, thumbWidth, thumbHeight, updateDate);
                            this.UpdateMemoryCache(thumb);
                            return thumb;
                        }
                    }
                    else
                    {
                        // サムネイルを作成します。
                        var thumb = this.CreateDBFileCache(
                            filePath, thumbWidth, thumbHeight, updateDate);
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
                        var thumb = this.UpdateDBFileCache(
                            filePath, thumbWidth, thumbHeight, updateDate);
                        this.UpdateMemoryCache(thumb);
                        return thumb;
                    }
                }
                else
                {
                    // サムネイルを作成します。
                    var updateDate = FileUtil.GetUpdateDate(filePath);
                    var thumb = this.CreateDBFileCache(
                        filePath, thumbWidth, thumbHeight, updateDate);
                    this.UpdateMemoryCache(thumb);
                    return thumb;
                }
            }
        }

        private ThumbnailCacheEntity GetOrCreateDirectoryCache(
            string directoryPath, int thumbWidth, int thumbHeight)
        {
            var memCache = this.GetMemoryCache(directoryPath);
            if (memCache != ThumbnailCacheEntity.EMPTY)
            {
                var updateDate = FileUtil.GetUpdateDate(directoryPath);
                if (memCache.ThumbnailWidth >= thumbWidth &&
                    memCache.ThumbnailHeight >= thumbHeight &&
                    memCache.FileUpdatedate >= updateDate)
                {
                    // メモリキャッシュを返します。
                    return memCache;
                }
                else
                {
                    var dbCache = this.GetDBCache(directoryPath);
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
                            var thumb = this.UpdateDBDirectoryCache(
                                directoryPath, thumbWidth, thumbHeight, updateDate);
                            this.UpdateMemoryCache(thumb);
                            return thumb;
                        }
                    }
                    else
                    {
                        // サムネイルを作成します。
                        var thumb = this.CreateDBDirectoryCache(
                            directoryPath, thumbWidth, thumbHeight, updateDate);
                        this.UpdateMemoryCache(thumb);
                        return thumb;
                    }
                }
            }
            else
            {
                var dbCache = this.GetDBCache(directoryPath);
                if (dbCache != ThumbnailCacheEntity.EMPTY)
                {
                    var updateDate = FileUtil.GetUpdateDate(directoryPath);

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
                        var thumb = this.UpdateDBDirectoryCache(
                            directoryPath, thumbWidth, thumbHeight, updateDate);
                        if (thumb != ThumbnailCacheEntity.EMPTY)
                        {
                            this.UpdateMemoryCache(thumb);
                        }

                        return thumb;
                    }
                }
                else
                {
                    // サムネイルを作成します。
                    var updateDate = FileUtil.GetUpdateDate(directoryPath);
                    var thumb = this.CreateDBDirectoryCache(
                        directoryPath, thumbWidth, thumbHeight, updateDate);
                    if (thumb != ThumbnailCacheEntity.EMPTY)
                    {
                        this.UpdateMemoryCache(thumb);
                    }

                    return thumb;
                }
            }
        }

        private ThumbnailCacheEntity GetDBCache(string filePath)
        {
            using (var con = Instance<IThumbnailDB>.Value.Connect())
            {
                var sql = new ThumbnailReadByFileSql(filePath);
                var dto = con.ReadLine(sql);

                if (dto != null)
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

                    var thumbBufferFile = GetThumbnailBufferFilePath(dto.ThumbnailID);
                    thumb.ThumbnailBuffer = this._fileAppender.Read(
                        thumbBufferFile, dto.ThumbnailStartPoint, dto.ThumbnailSize);

                    return thumb;
                }
                else
                {
                    return ThumbnailCacheEntity.EMPTY;
                }
            }
        }

        private ThumbnailCacheEntity CreateDBFileCache(
            string filePath, int thumbWidth, int thumbHeight, DateTime fileUpdateDate)
        {
            using (var srcImg = ImageUtil.ReadImageFile(filePath))
            using (var thumbImg = ThumbnailUtil.CreateThumbnail(srcImg, thumbWidth, thumbHeight))
            {
                var thumbBin = ThumbnailUtil.ToCompressionBinary(thumbImg);
                using (var con = Instance<IThumbnailDB>.Value.ConnectWithTransaction())
                {
                    var count = con.ReadValue<long>(
                        new ThumbnailCountByFileSql(filePath));
                    if (count < 1)
                    {
                        var thumbID = GetCurrentThumbnailBufferID(con);
                        var thumbFile = GetThumbnailBufferFilePath(thumbID);
                        var thumbStartPoint = this._fileAppender.Append(thumbFile, thumbBin);

                        var sql = new ThumbnailCreationSql(
                            filePath,
                            thumbID,
                            thumbStartPoint,
                            thumbBin.Length,
                            thumbWidth,
                            thumbHeight,
                            srcImg.Width,
                            srcImg.Height,
                            fileUpdateDate);
                        con.Update(sql);

                        con.Commit();
                    }
                }

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

        private ThumbnailCacheEntity UpdateDBFileCache(
            string filePath, int thumbWidth, int thumbHeight, DateTime fileUpdateDate)
        {
            using (var srcImg = ImageUtil.ReadImageFile(filePath))
            using (var thumbImg = ThumbnailUtil.CreateThumbnail(srcImg, thumbWidth, thumbHeight))
            {
                var thumbBin = ThumbnailUtil.ToCompressionBinary(thumbImg);
                using (var con = Instance<IThumbnailDB>.Value.ConnectWithTransaction())
                {
                    var thumbID = GetCurrentThumbnailBufferID(con);
                    var thumbFile = GetThumbnailBufferFilePath(thumbID);
                    var thumbStartPoint = this._fileAppender.Append(thumbFile, thumbBin);

                    var sql = new ThumbnailUpdateSql(
                        filePath,
                        thumbID,
                        thumbStartPoint,
                        thumbBin.Length,
                        thumbWidth,
                        thumbHeight,
                        srcImg.Width,
                        srcImg.Height,
                        fileUpdateDate);
                    con.Update(sql);

                    con.Commit();
                }

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

        private ThumbnailCacheEntity CreateDBDirectoryCache(
            string directoryPath, int thumbWidth, int thumbHeight, DateTime directoryUpdateDate)
        {
            var thumbFilePath = ImageUtil.GetFirstImageFilePath(directoryPath);
            if (string.IsNullOrEmpty(thumbFilePath))
            {
                return ThumbnailCacheEntity.EMPTY;
            }

            using (var srcImg = ImageUtil.ReadImageFile(thumbFilePath))
            using (var thumbImg = ThumbnailUtil.CreateThumbnail(srcImg, thumbWidth, thumbHeight))
            {
                var thumbBin = ThumbnailUtil.ToCompressionBinary(thumbImg);
                using (var con = Instance<IThumbnailDB>.Value.ConnectWithTransaction())
                {
                    var count = con.ReadValue<long>(
                        new ThumbnailCountByFileSql(directoryPath));
                    if (count < 1)
                    {
                        var thumbID = GetCurrentThumbnailBufferID(con);
                        var thumbFile = GetThumbnailBufferFilePath(thumbID);
                        var thumbStartPoint = this._fileAppender.Append(thumbFile, thumbBin);

                        var sql = new ThumbnailCreationSql(
                            directoryPath,
                            thumbID,
                            thumbStartPoint,
                            thumbBin.Length,
                            thumbWidth,
                            thumbHeight,
                            srcImg.Width,
                            srcImg.Height,
                            directoryUpdateDate);
                        con.Update(sql);

                        con.Commit();
                    }
                }

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

        private ThumbnailCacheEntity UpdateDBDirectoryCache(
            string directoryPath, int thumbWidth, int thumbHeight, DateTime directoryUpdateDate)
        {
            var thumbFilePath = ImageUtil.GetFirstImageFilePath(directoryPath);
            if (string.IsNullOrEmpty(thumbFilePath))
            {
                return ThumbnailCacheEntity.EMPTY;
            }

            using (var srcImg = ImageUtil.ReadImageFile(thumbFilePath))
            {
                using (var thumbImg = ThumbnailUtil.CreateThumbnail(srcImg, thumbWidth, thumbHeight))
                {
                    var thumbBin = ThumbnailUtil.ToCompressionBinary(thumbImg);
                    using (var con = Instance<IThumbnailDB>.Value.ConnectWithTransaction())
                    {
                        var thumbID = GetCurrentThumbnailBufferID(con);
                        var thumbFile = GetThumbnailBufferFilePath(thumbID);
                        var thumbStartPoint = this._fileAppender.Append(thumbFile, thumbBin);

                        var sql = new ThumbnailUpdateSql(
                            directoryPath,
                            thumbID,
                            thumbStartPoint,
                            thumbBin.Length,
                            thumbWidth,
                            thumbHeight,
                            srcImg.Width,
                            srcImg.Height,
                            directoryUpdateDate);
                        con.Update(sql);

                        con.Commit();
                    }

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
            lock (this._cacheLock)
            {
                if (this._cacheDictionary.TryGetValue(filePath, out var cach))
                {
                    return cach;
                }
                else
                {
                    return ThumbnailCacheEntity.EMPTY;
                }
            }
        }

        private void UpdateMemoryCache(ThumbnailCacheEntity thumb)
        {
            if (thumb == ThumbnailCacheEntity.EMPTY)
            {
                throw new ArgumentException("サムネイルインスタンスが空です。", nameof(thumb));
            }

            lock (this._cacheLock)
            {
                if (this._cacheDictionary.TryGetValue(thumb.FilePath, out var oldCache))
                {
                    this._cacheList.RemoveAll(_ => _.FilePath == oldCache.FilePath);
                    this._cacheList.Add(thumb);
                    this._cacheDictionary[thumb.FilePath] = thumb;
                }
                else
                {
                    if (this._cacheList.Count == CACHE_CAPACITY)
                    {
                        var firstCache = this._cacheList[0];
                        this._cacheList.RemoveAt(0);
                        this._cacheDictionary.Remove(firstCache.FilePath);
                    }

                    this._cacheList.Add(thumb);
                    this._cacheDictionary.Add(thumb.FilePath, thumb);
                }
            }
        }
    }
}
