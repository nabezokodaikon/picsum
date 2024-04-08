using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using PicSum.Task.Entities;
using SWF.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using System.Threading;

namespace PicSum.Task.Logics
{
    /// <summary>
    /// サムネイルを読込みます。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetThumbnailLogic(IAsyncTask task)
        : AbstractAsyncLogic(task)
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
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }
            else if (FileUtil.IsFile(filePath))
            {
                if (!FileUtil.IsImageFile(filePath))
                {
                    return null;
                }

                var cash = this.GetFileCash(filePath, thumbWidth, thumbHeight);
                if (cash != null)
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
                    return null;
                }
            }
            else
            {
                var cash = this.GetDirectoryCash(filePath, thumbWidth, thumbHeight);
                if (cash != null)
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
                    return null;
                }
            }
        }

        private ThumbnailBufferEntity GetFileCash(string filePath, int thumbWidth, int thumbHeight)
        {
            var memCash = GetMemoryCash(filePath);
            if (memCash != null)
            {
                var updateDate = FileUtil.GetUpdateDate(filePath);
                if (!updateDate.HasValue)
                {
                    return null;
                }

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
                    if (dbCash != null)
                    {
                        if (dbCash.ThumbnailWidth >= thumbWidth &&
                            dbCash.ThumbnailHeight >= thumbHeight &&
                            dbCash.FileUpdatedate >= updateDate.Value)
                        {
                            // DBキャッシュを返します。
                            UpdateMemoryCash(dbCash);
                            return dbCash;
                        }
                        else
                        {
                            // サムネイルを更新します。
                            var thumb = this.UpdateDBFileCash(filePath, thumbWidth, thumbHeight, updateDate.Value);
                            UpdateMemoryCash(thumb);
                            return thumb;
                        }
                    }
                    else
                    {
                        // サムネイルを作成します。
                        var thumb = this.CreateDBFileCash(filePath, thumbWidth, thumbHeight, updateDate.Value);
                        UpdateMemoryCash(thumb);
                        return thumb;
                    }
                }
            }
            else
            {
                var dbCash = this.GetDBCash(filePath);
                if (dbCash != null)
                {
                    var updateDate = FileUtil.GetUpdateDate(filePath);
                    if (!updateDate.HasValue)
                    {
                        return null;
                    }

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
                        var thumb = this.UpdateDBFileCash(filePath, thumbWidth, thumbHeight, updateDate.Value);
                        UpdateMemoryCash(thumb);
                        return thumb;
                    }
                }
                else
                {
                    // サムネイルを作成します。
                    var updateDate = FileUtil.GetUpdateDate(filePath);
                    if (!updateDate.HasValue)
                    {
                        return null;
                    }

                    var thumb = this.CreateDBFileCash(filePath, thumbWidth, thumbHeight, updateDate.Value);
                    UpdateMemoryCash(thumb);
                    return thumb;
                }
            }
        }

        private ThumbnailBufferEntity GetDirectoryCash(string filePath, int thumbWidth, int thumbHeight)
        {
            var memCash = GetMemoryCash(filePath);
            if (memCash != null)
            {
                var updateDate = FileUtil.GetUpdateDate(filePath);
                if (!updateDate.HasValue)
                {
                    return null;
                }

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
                    if (dbCash != null)
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
                                var thumb = this.UpdateDBDirectoryCash(filePath, thumbFile, thumbWidth, thumbHeight, updateDate.Value);
                                UpdateMemoryCash(thumb);
                                return thumb;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    else
                    {
                        var thumbFile = FileUtil.GetFirstImageFilePath(filePath);
                        if (!string.IsNullOrEmpty(thumbFile))
                        {
                            // サムネイルを作成します。
                            var thumb = this.CreateDBDirectoryCash(filePath, thumbFile, thumbWidth, thumbHeight, updateDate.Value);
                            UpdateMemoryCash(thumb);
                            return thumb;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            else
            {
                var dbCash = this.GetDBCash(filePath);
                if (dbCash != null)
                {
                    var updateDate = FileUtil.GetUpdateDate(filePath);
                    if (!updateDate.HasValue)
                    {
                        return null;
                    }

                    if (dbCash.ThumbnailWidth >= thumbWidth &&
                        dbCash.ThumbnailHeight >= thumbHeight &&
                        dbCash.FileUpdatedate >= updateDate.Value)
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
                            var thumb = this.UpdateDBDirectoryCash(filePath, thumbFile, thumbWidth, thumbHeight, updateDate.Value);
                            UpdateMemoryCash(thumb);
                            return thumb;
                        }
                        else
                        {
                            return null;
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
                        if (!updateDate.HasValue)
                        {
                            return null;
                        }

                        var thumb = this.CreateDBDirectoryCash(filePath, thumbFile, thumbWidth, thumbHeight, updateDate.Value);
                        UpdateMemoryCash(thumb);
                        return thumb;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        #region DBキャッシュ操作メソッド

        private string GetThumbnailBufferFilePath(int id)
        {
            var dbDir = Directory.GetParent(DatabaseManager<ThumbnailConnection>.DBFilePath).FullName;
            return Path.Combine(dbDir, $"{id}.thumbnail");
        }

        private int GetCurrentThumbnailBufferID()
        {
            const int BUFFER_FILE_MAX_SIZE = 1000 * 1000 * 100;

            var id = (int)DatabaseManager<ThumbnailConnection>.ReadValue<long>(new ReadThumbnailIDSql());
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
                    DatabaseManager<ThumbnailConnection>.Update(new UpdateThumbnailIDSql());
                    var newID = (int)DatabaseManager<ThumbnailConnection>.ReadValue<long>(new ReadThumbnailIDSql());
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
            var sql = new ReadThumbnailByFileSql(filePath);
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
                return null;
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

                    var sql = new CreationThumbnailSql(
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

                    var sql = new UpdateThumbnailSql(
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

                    var sql = new CreationThumbnailSql(
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

                    var sql = new UpdateThumbnailSql(
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
                if (CASH_DICTIONARY.TryGetValue(filePath, out ThumbnailBufferEntity cach))
                {
                    return cach;
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                CASH_LOCK.ExitReadLock();
            }
        }

        private static void UpdateMemoryCash(ThumbnailBufferEntity thumb)
        {
            CASH_LOCK.EnterWriteLock();

            try
            {
                if (CASH_DICTIONARY.TryGetValue(thumb.FilePath, out ThumbnailBufferEntity dicCash))
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
