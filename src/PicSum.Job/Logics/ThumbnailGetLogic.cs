using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Job.AsyncJob;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using PicSum.Job.Entities;
using SWF.Common;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// サムネイルを読込みます。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class ThumbnailGetLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        private const int CASH_CAPACITY = 1000;
        private static readonly List<ThumbnailBufferEntity> CASH_LIST = new(CASH_CAPACITY);
        private static readonly Dictionary<string, ThumbnailBufferEntity> CASH_DICTIONARY = new(CASH_CAPACITY);
        private static readonly ReaderWriterLockSlim CASH_LOCK = new();

        /// <summary>
        /// 静的リソースを解放します。
        /// </summary>
        public static void DisposeStaticResouces()
        {
            CASH_LOCK.Dispose();
        }

        public ThumbnailBufferEntity Execute(string filePath, int thumbWidth, int thumbHeight)
        {
            ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));

            if (string.IsNullOrEmpty(filePath))
            {
                return ThumbnailBufferEntity.EMPTY;
            }
            else if (FileUtil.IsFile(filePath))
            {
                if (!FileUtil.IsImageFile(filePath))
                {
                    return ThumbnailBufferEntity.EMPTY;
                }

                var cash = this.GetFileCash(filePath, thumbWidth, thumbHeight);
                if (cash != ThumbnailBufferEntity.EMPTY)
                {
                    if (cash.ThumbnailWidth > thumbWidth || cash.ThumbnailHeight > thumbHeight)
                    {
                        using (var cashThumb = ImageUtil.ToImage(cash.ThumbnailBuffer))
                        {
                            using (var newThumb = ThumbnailUtil.CreateThumbnail(cashThumb, thumbWidth, thumbHeight))
                            {
                                var thumb = new ThumbnailBufferEntity
                                {
                                    FilePath = cash.FilePath,
                                    ThumbnailBuffer = ImageUtil.ToCompressionBinary(newThumb),
                                    ThumbnailWidth = thumbWidth,
                                    ThumbnailHeight = thumbHeight,
                                    SourceWidth = cash.SourceWidth,
                                    SourceHeight = cash.SourceHeight,
                                    FileUpdatedate = cash.FileUpdatedate
                                };
                                return thumb;
                            }
                        }
                    }
                    else
                    {
                        return cash;
                    }
                }
                else
                {
                    return ThumbnailBufferEntity.EMPTY;
                }
            }
            else
            {
                var cash = this.GetDirectoryCash(filePath, thumbWidth, thumbHeight);
                if (cash != ThumbnailBufferEntity.EMPTY)
                {
                    if (cash.ThumbnailWidth > thumbWidth || cash.ThumbnailHeight > thumbHeight)
                    {
                        using (var cashThumb = ImageUtil.ToImage(cash.ThumbnailBuffer))
                        {
                            using (var newThumb = ThumbnailUtil.CreateThumbnail(cashThumb, thumbWidth, thumbHeight))
                            {
                                var thumb = new ThumbnailBufferEntity
                                {
                                    FilePath = cash.FilePath,
                                    ThumbnailBuffer = ImageUtil.ToCompressionBinary(newThumb),
                                    ThumbnailWidth = thumbWidth,
                                    ThumbnailHeight = thumbHeight,
                                    SourceWidth = cash.SourceWidth,
                                    SourceHeight = cash.SourceHeight,
                                    FileUpdatedate = cash.FileUpdatedate
                                };
                                return thumb;
                            }
                        }
                    }
                    else
                    {
                        return cash;
                    }
                }
                else
                {
                    return ThumbnailBufferEntity.EMPTY;
                }
            }
        }

        private ThumbnailBufferEntity GetFileCash(string filePath, int thumbWidth, int thumbHeight)
        {
            var memCash = GetMemoryCash(filePath);
            if (memCash != ThumbnailBufferEntity.EMPTY)
            {
                var updateDate = FileUtil.GetUpdateDate(filePath);
                if (memCash.ThumbnailWidth >= thumbWidth &&
                    memCash.ThumbnailHeight >= thumbHeight &&
                    memCash.FileUpdatedate >= updateDate)
                {
                    // メモリキャッシュを返します。
                    return memCash;
                }
                else
                {
                    var dbCash = this.GetDBCash(filePath);
                    if (dbCash != ThumbnailBufferEntity.EMPTY)
                    {
                        if (dbCash.ThumbnailWidth >= thumbWidth &&
                            dbCash.ThumbnailHeight >= thumbHeight &&
                            dbCash.FileUpdatedate >= updateDate)
                        {
                            // DBキャッシュを返します。
                            UpdateMemoryCash(dbCash);
                            return dbCash;
                        }
                        else
                        {
                            // サムネイルを更新します。
                            var thumb = this.UpdateDBFileCash(filePath, thumbWidth, thumbHeight, updateDate);
                            UpdateMemoryCash(thumb);
                            return thumb;
                        }
                    }
                    else
                    {
                        // サムネイルを作成します。
                        var thumb = this.CreateDBFileCash(filePath, thumbWidth, thumbHeight, updateDate);
                        UpdateMemoryCash(thumb);
                        return thumb;
                    }
                }
            }
            else
            {
                var dbCash = this.GetDBCash(filePath);
                if (dbCash != ThumbnailBufferEntity.EMPTY)
                {
                    var updateDate = FileUtil.GetUpdateDate(filePath);

                    if (dbCash.ThumbnailWidth >= thumbWidth &&
                        dbCash.ThumbnailHeight >= thumbHeight &&
                        dbCash.FileUpdatedate >= updateDate)
                    {
                        // DBキャッシュを返します。
                        UpdateMemoryCash(dbCash);
                        return dbCash;
                    }
                    else
                    {
                        // サムネイルを更新します。
                        var thumb = this.UpdateDBFileCash(filePath, thumbWidth, thumbHeight, updateDate);
                        UpdateMemoryCash(thumb);
                        return thumb;
                    }
                }
                else
                {
                    // サムネイルを作成します。
                    var updateDate = FileUtil.GetUpdateDate(filePath);
                    var thumb = this.CreateDBFileCash(filePath, thumbWidth, thumbHeight, updateDate);
                    UpdateMemoryCash(thumb);
                    return thumb;
                }
            }
        }

        private ThumbnailBufferEntity GetDirectoryCash(string filePath, int thumbWidth, int thumbHeight)
        {
            var memCash = GetMemoryCash(filePath);
            if (memCash != ThumbnailBufferEntity.EMPTY)
            {
                var updateDate = FileUtil.GetUpdateDate(filePath);

                if (memCash.ThumbnailWidth >= thumbWidth &&
                    memCash.ThumbnailHeight >= thumbHeight &&
                    memCash.FileUpdatedate >= updateDate)
                {
                    // メモリキャッシュを返します。
                    return memCash;
                }
                else
                {
                    var dbCash = this.GetDBCash(filePath);
                    if (dbCash != ThumbnailBufferEntity.EMPTY)
                    {
                        if (dbCash.ThumbnailWidth >= thumbWidth &&
                            dbCash.ThumbnailHeight >= thumbHeight &&
                            dbCash.FileUpdatedate >= updateDate)
                        {
                            // DBキャッシュを返します。
                            UpdateMemoryCash(dbCash);
                            return dbCash;
                        }
                        else
                        {
                            var thumbFile = FileUtil.GetFirstImageFilePath(filePath);
                            if (!string.IsNullOrEmpty(thumbFile))
                            {
                                // サムネイルを更新します。
                                var thumb = this.UpdateDBDirectoryCash(filePath, thumbFile, thumbWidth, thumbHeight, updateDate);
                                UpdateMemoryCash(thumb);
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
                            var thumb = this.CreateDBDirectoryCash(filePath, thumbFile, thumbWidth, thumbHeight, updateDate);
                            UpdateMemoryCash(thumb);
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
                var dbCash = this.GetDBCash(filePath);
                if (dbCash != ThumbnailBufferEntity.EMPTY)
                {
                    var updateDate = FileUtil.GetUpdateDate(filePath);

                    if (dbCash.ThumbnailWidth >= thumbWidth &&
                        dbCash.ThumbnailHeight >= thumbHeight &&
                        dbCash.FileUpdatedate >= updateDate)
                    {
                        // DBキャッシュを返します。
                        UpdateMemoryCash(dbCash);
                        return dbCash;
                    }
                    else
                    {
                        var thumbFile = FileUtil.GetFirstImageFilePath(filePath);
                        if (!string.IsNullOrEmpty(thumbFile))
                        {
                            // サムネイルを更新します。                                
                            var thumb = this.UpdateDBDirectoryCash(filePath, thumbFile, thumbWidth, thumbHeight, updateDate);
                            UpdateMemoryCash(thumb);
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
                        var thumb = this.CreateDBDirectoryCash(filePath, thumbFile, thumbWidth, thumbHeight, updateDate);
                        UpdateMemoryCash(thumb);
                        return thumb;
                    }
                    else
                    {
                        return ThumbnailBufferEntity.EMPTY;
                    }
                }
            }
        }

        #region DBキャッシュ操作メソッド

        private string GetThumbnailBufferFilePath(int id)
        {
#pragma warning disable CS8602
            var dbDir = Directory.GetParent(DatabaseManager<ThumbnailConnection>.DBFilePath).FullName;
#pragma warning restore CS8602
            return Path.Combine(dbDir, $"{id}.thumbnail");
        }

        private int GetCurrentThumbnailBufferID()
        {
            const int BUFFER_FILE_MAX_SIZE = 1000 * 1000 * 100;

            var id = (int)DatabaseManager<ThumbnailConnection>.ReadValue<long>(new ThumbnailIDReadSql());
            var thumbFile = this.GetThumbnailBufferFilePath(id);
            if (!File.Exists(thumbFile))
            {
                return id;
            }

            using (var fs = new FileStream(thumbFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var size = fs.Length;
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
        }

        private byte[] GetThumbnailBuffer(string filePath, int startPoint, int size)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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
            using (var fs = new FileStream(thumbFile, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                var offset = (int)fs.Length;
                fs.Seek(fs.Length, SeekOrigin.Begin);
                fs.Write(buffer, 0, buffer.Length);
                return offset;
            }
        }

        private ThumbnailBufferEntity GetDBCash(string filePath)
        {
            var sql = new ThumbnailReadByFileSql(filePath);
            var dto = DatabaseManager<ThumbnailConnection>.ReadLine<ThumbnailDto>(sql);
            if (dto != null)
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
                thumb.ThumbnailBuffer = this.GetThumbnailBuffer(thumbBufferFile, dto.ThumbnailStartPoint, dto.ThumbnailSize);

                return thumb;
            }
            else
            {
                return ThumbnailBufferEntity.EMPTY;
            }
        }

        private ThumbnailBufferEntity CreateDBFileCash(
            string filePath, int thumbWidth, int thumbHeight, DateTime fileUpdateDate)
        {
            using (var srcImg = ImageUtil.ReadImageFile(filePath))
            {
                using (var thumbImg = ThumbnailUtil.CreateThumbnail(srcImg, thumbWidth, thumbHeight))
                {
                    var thumbBin = ImageUtil.ToCompressionBinary(thumbImg);
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

        private ThumbnailBufferEntity UpdateDBFileCash(string filePath, int thumbWidth, int thumbHeight, DateTime fileUpdateDate)
        {
            using (var srcImg = ImageUtil.ReadImageFile(filePath))
            {
                using (var thumbImg = ThumbnailUtil.CreateThumbnail(srcImg, thumbWidth, thumbHeight))
                {
                    var thumbBin = ImageUtil.ToCompressionBinary(thumbImg);
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

        private ThumbnailBufferEntity CreateDBDirectoryCash(string directoryPath, string thumbFilePath, int thumbWidth, int thumbHeight, DateTime directoryUpdateDate)
        {
            using (var srcImg = ImageUtil.ReadImageFile(thumbFilePath))
            {
                using (var thumbImg = ThumbnailUtil.CreateThumbnail(srcImg, thumbWidth, thumbHeight))
                {
                    var thumbBin = ImageUtil.ToCompressionBinary(thumbImg);
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

        private ThumbnailBufferEntity UpdateDBDirectoryCash(string directoryPath, string thumbFilePath, int thumbWidth, int thumbHeight, DateTime directoryUpdateDate)
        {
            using (var srcImg = ImageUtil.ReadImageFile(thumbFilePath))
            {
                using (var thumbImg = ThumbnailUtil.CreateThumbnail(srcImg, thumbWidth, thumbHeight))
                {
                    var thumbBin = ImageUtil.ToCompressionBinary(thumbImg);
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

        #endregion

        #region メモリーキャッシュ操作メソッド

        private static ThumbnailBufferEntity GetMemoryCash(string filePath)
        {
            CASH_LOCK.EnterReadLock();

            try
            {
                if (CASH_DICTIONARY.TryGetValue(filePath, out var cach))
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
                CASH_LOCK.ExitReadLock();
            }
        }

        private static void UpdateMemoryCash(ThumbnailBufferEntity thumb)
        {
            if (thumb.FilePath == null)
            {
                throw new ArgumentException("サムネイルのファイルパスがNULLです。", nameof(thumb));
            }

            CASH_LOCK.EnterWriteLock();

            try
            {
                if (CASH_DICTIONARY.TryGetValue(thumb.FilePath, out var dicCash))
                {
                    CASH_LIST.Remove(dicCash);
                    CASH_LIST.Add(thumb);
                    CASH_DICTIONARY[thumb.FilePath] = thumb;
                }
                else
                {
                    if (CASH_LIST.Count == CASH_LIST.Capacity)
                    {
                        var cash = CASH_LIST[0];
                        CASH_LIST.Remove(cash);
                        CASH_DICTIONARY.Remove(thumb.FilePath);
                    }

                    CASH_LIST.Add(thumb);
                    CASH_DICTIONARY.Add(thumb.FilePath, thumb);
                }
            }
            finally
            {
                CASH_LOCK.ExitWriteLock();
            }
        }

        #endregion
    }
}
