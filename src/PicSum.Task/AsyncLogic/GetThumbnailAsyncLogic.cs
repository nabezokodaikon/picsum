using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using PicSum.Task.Entity;
using SWF.Common;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// サムネイルを読込みます。
    /// </summary>
    public class GetThumbnailAsyncLogic : AsyncLogicBase
    {
        private const int CashCapacity = 1000;
        private static List<ThumbnailBufferEntity> _cashList = new List<ThumbnailBufferEntity>(CashCapacity);
        private static Dictionary<string, ThumbnailBufferEntity> _cashDic = new Dictionary<string, ThumbnailBufferEntity>(CashCapacity);
        private static readonly ReaderWriterLockSlim _cashLock = new ReaderWriterLockSlim();

        /// <summary>
        /// 静的リソースを解放します。
        /// </summary>
        public static void DisposeStaticResouces()
        {
            _cashLock.Dispose();
        }

        public GetThumbnailAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

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

                ThumbnailBufferEntity cash = getFileCash(filePath, thumbWidth, thumbHeight);
                if (cash != null)
                {
                    if (cash.ThumbnailWidth > thumbWidth || cash.ThumbnailHeight > thumbHeight)
                    {
                        using (var cashThumb = ImageUtil.ToImage(cash.ThumbnailBuffer))
                        {
                            using (Image newThumb = ThumbnailUtil.CreateThumbnail(cashThumb, thumbWidth, thumbHeight))
                            {
                                ThumbnailBufferEntity thumb = new ThumbnailBufferEntity();
                                thumb.FilePath = cash.FilePath;
                                thumb.ThumbnailBuffer = ImageUtil.ToCompressionBinary(newThumb);
                                thumb.ThumbnailWidth = thumbWidth;
                                thumb.ThumbnailHeight = thumbHeight;
                                thumb.SourceWidth = cash.SourceWidth;
                                thumb.SourceHeight = cash.SourceHeight;
                                thumb.FileUpdatedate = cash.FileUpdatedate;
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
                ThumbnailBufferEntity cash = getDirectoryCash(filePath, thumbWidth, thumbHeight);
                if (cash != null)
                {
                    if (cash.ThumbnailWidth > thumbWidth || cash.ThumbnailHeight > thumbHeight)
                    {
                        using (Image cashThumb = ImageUtil.ToImage(cash.ThumbnailBuffer))
                        {
                            using (Image newThumb = ThumbnailUtil.CreateThumbnail(cashThumb, thumbWidth, thumbHeight))
                            {
                                ThumbnailBufferEntity thumb = new ThumbnailBufferEntity();
                                thumb.FilePath = cash.FilePath;
                                thumb.ThumbnailBuffer = ImageUtil.ToCompressionBinary(newThumb);
                                thumb.ThumbnailWidth = thumbWidth;
                                thumb.ThumbnailHeight = thumbHeight;
                                thumb.SourceWidth = cash.SourceWidth;
                                thumb.SourceHeight = cash.SourceHeight;
                                thumb.FileUpdatedate = cash.FileUpdatedate;
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

        private ThumbnailBufferEntity getFileCash(string filePath, int thumbWidth, int thumbHeight)
        {
            ThumbnailBufferEntity memCash = getMemoryCash(filePath);
            if (memCash != null)
            {
                var createDate = FileUtil.GetCreateDate(filePath);
                var updateDate = FileUtil.GetUpdateDate(filePath);
                if (!createDate.HasValue || !updateDate.HasValue)
                {
                    return null;
                }

                if (memCash.ThumbnailWidth >= thumbWidth &&
                    memCash.ThumbnailHeight >= thumbHeight &&
                    memCash.FileUpdatedate >= createDate &&
                    memCash.FileUpdatedate >= updateDate)
                {
                    // メモリキャッシュを返します。
                    return memCash;
                }
                else
                {
                    ThumbnailBufferEntity dbCash = getDBCash(filePath);
                    if (dbCash != null)
                    {
                        if (dbCash.ThumbnailWidth >= thumbWidth &&
                            dbCash.ThumbnailHeight >= thumbHeight &&
                            dbCash.FileUpdatedate >= createDate.Value &&
                            dbCash.FileUpdatedate >= updateDate.Value)
                        {
                            // DBキャッシュを返します。
                            updateMemoryCash(dbCash);
                            return dbCash;
                        }
                        else
                        {
                            // サムネイルを更新します。
                            ThumbnailBufferEntity thumb = updateDBFileCash(filePath, thumbWidth, thumbHeight, updateDate.Value);
                            updateMemoryCash(thumb);
                            return thumb;
                        }
                    }
                    else
                    {
                        // サムネイルを作成します。
                        ThumbnailBufferEntity thumb = createDBFileCash(filePath, thumbWidth, thumbHeight, updateDate.Value);
                        updateMemoryCash(thumb);
                        return thumb;
                    }
                }
            }
            else
            {
                ThumbnailBufferEntity dbCash = getDBCash(filePath);
                if (dbCash != null)
                {
                    var createDate = FileUtil.GetCreateDate(filePath);
                    var updateDate = FileUtil.GetUpdateDate(filePath);
                    if (!createDate.HasValue || !updateDate.HasValue)
                    {
                        return null;
                    }

                    if (dbCash.ThumbnailWidth >= thumbWidth &&
                        dbCash.ThumbnailHeight >= thumbHeight &&
                        dbCash.FileUpdatedate >= createDate &&
                        dbCash.FileUpdatedate >= updateDate)
                    {
                        // DBキャッシュを返します。
                        updateMemoryCash(dbCash);
                        return dbCash;
                    }
                    else
                    {
                        // サムネイルを更新します。
                        ThumbnailBufferEntity thumb = updateDBFileCash(filePath, thumbWidth, thumbHeight, updateDate.Value);
                        updateMemoryCash(thumb);
                        return thumb;
                    }
                }
                else
                {
                    // サムネイルを作成します。
                    var updateDate = FileUtil.GetCreateDate(filePath);
                    if (!updateDate.HasValue)
                    {
                        return null;
                    }

                    ThumbnailBufferEntity thumb = createDBFileCash(filePath, thumbWidth, thumbHeight, updateDate.Value);
                    updateMemoryCash(thumb);
                    return thumb;
                }
            }
        }

        private ThumbnailBufferEntity getDirectoryCash(string filePath, int thumbWidth, int thumbHeight)
        {
            ThumbnailBufferEntity memCash = getMemoryCash(filePath);
            if (memCash != null)
            {
                var createDate = FileUtil.GetCreateDate(filePath);
                var updateDate = FileUtil.GetUpdateDate(filePath);
                if (!createDate.HasValue || !updateDate.HasValue)
                {
                    return null;
                }

                if (memCash.ThumbnailWidth >= thumbWidth &&
                    memCash.ThumbnailHeight >= thumbHeight &&
                    memCash.FileUpdatedate >= createDate &&
                    memCash.FileUpdatedate >= updateDate)
                {
                    // メモリキャッシュを返します。
                    return memCash;
                }
                else
                {
                    ThumbnailBufferEntity dbCash = getDBCash(filePath);
                    if (dbCash != null)
                    {
                        if (dbCash.ThumbnailWidth >= thumbWidth &&
                            dbCash.ThumbnailHeight >= thumbHeight &&
                            dbCash.FileUpdatedate >= createDate &&
                            dbCash.FileUpdatedate >= updateDate)
                        {
                            // DBキャッシュを返します。
                            updateMemoryCash(dbCash);
                            return dbCash;
                        }
                        else
                        {
                            string thumbFile = FileUtil.GetFirstImageFilePath(filePath);
                            if (!string.IsNullOrEmpty(thumbFile))
                            {
                                // サムネイルを更新します。
                                ThumbnailBufferEntity thumb = updateDBDirectoryCash(filePath, thumbFile, thumbWidth, thumbHeight, updateDate.Value);
                                updateMemoryCash(thumb);
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
                        string thumbFile = FileUtil.GetFirstImageFilePath(filePath);
                        if (!string.IsNullOrEmpty(thumbFile))
                        {
                            // サムネイルを作成します。
                            ThumbnailBufferEntity thumb = createDBDirectoryCash(filePath, thumbFile, thumbWidth, thumbHeight, updateDate.Value);
                            updateMemoryCash(thumb);
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
                ThumbnailBufferEntity dbCash = getDBCash(filePath);
                if (dbCash != null)
                {
                    var createDate = FileUtil.GetCreateDate(filePath);
                    var updateDate = FileUtil.GetUpdateDate(filePath);
                    if (!createDate.HasValue || !updateDate.HasValue)
                    {
                        return null;
                    }

                    if (dbCash.ThumbnailWidth >= thumbWidth &&
                        dbCash.ThumbnailHeight >= thumbHeight &&
                        dbCash.FileUpdatedate >= createDate.Value &&
                        dbCash.FileUpdatedate >= updateDate.Value)
                    {
                        // DBキャッシュを返します。
                        updateMemoryCash(dbCash);
                        return dbCash;
                    }
                    else
                    {
                        string thumbFile = FileUtil.GetFirstImageFilePath(filePath);
                        if (!string.IsNullOrEmpty(thumbFile))
                        {
                            // サムネイルを更新します。                                
                            ThumbnailBufferEntity thumb = updateDBDirectoryCash(filePath, thumbFile, thumbWidth, thumbHeight, updateDate.Value);
                            updateMemoryCash(thumb);
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
                    string thumbFile = FileUtil.GetFirstImageFilePath(filePath);
                    if (!string.IsNullOrEmpty(thumbFile))
                    {
                        var updateDate = FileUtil.GetUpdateDate(filePath);
                        if (!updateDate.HasValue)
                        {
                            return null;
                        }

                        ThumbnailBufferEntity thumb = createDBDirectoryCash(filePath, thumbFile, thumbWidth, thumbHeight, updateDate.Value);
                        updateMemoryCash(thumb);
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

        private ThumbnailBufferEntity getDBCash(string filePath)
        {
            ReadThumbnailByFileSql sql = new ReadThumbnailByFileSql(filePath);
            ThumbnailDto dto = DatabaseManager<ThumbnailConnection>.ReadLine<ThumbnailDto>(sql);
            if (dto != null)
            {
                ThumbnailBufferEntity thumb = new ThumbnailBufferEntity();
                thumb.FilePath = dto.FilePath;
                thumb.ThumbnailBuffer = dto.ThumbnailBuffer;
                thumb.ThumbnailWidth = dto.ThumbnailWidth;
                thumb.ThumbnailHeight = dto.ThumbnailHeight;
                thumb.SourceWidth = dto.SourceWidth;
                thumb.SourceHeight = dto.SourceHeight;
                thumb.FileUpdatedate = dto.FileUpdatedate;
                return thumb;
            }
            else
            {
                return null;
            }
        }

        private ThumbnailBufferEntity createDBFileCash(string filePath, int thumbWidth, int thumbHeight, DateTime fileUpdateDate)
        {
            using (Image srcImg = ImageUtil.ReadImageFile(filePath))
            {
                using (Image thumbImg = ThumbnailUtil.CreateThumbnail(srcImg, thumbWidth, thumbHeight))
                {
                    byte[] thumbBin = ImageUtil.ToCompressionBinary(thumbImg);

                    CreationThumbnailSql sql = new CreationThumbnailSql(filePath, thumbBin, thumbWidth, thumbHeight, srcImg.Width, srcImg.Height, fileUpdateDate);
                    DatabaseManager<ThumbnailConnection>.Update(sql);

                    ThumbnailBufferEntity thumb = new ThumbnailBufferEntity();
                    thumb.FilePath = filePath;
                    thumb.ThumbnailBuffer = thumbBin;
                    thumb.ThumbnailWidth = thumbWidth;
                    thumb.ThumbnailHeight = thumbHeight;
                    thumb.SourceWidth = srcImg.Width;
                    thumb.SourceHeight = srcImg.Height;
                    thumb.FileUpdatedate = fileUpdateDate;

                    return thumb;
                }
            }
        }

        private ThumbnailBufferEntity updateDBFileCash(string filePath, int thumbWidth, int thumbHeight, DateTime fileUpdateDate)
        {
            using (Image srcImg = ImageUtil.ReadImageFile(filePath))
            {
                using (Image thumbImg = ThumbnailUtil.CreateThumbnail(srcImg, thumbWidth, thumbHeight))
                {
                    byte[] thumbBin = ImageUtil.ToCompressionBinary(thumbImg);

                    UpdateThumbnailSql sql = new UpdateThumbnailSql(filePath, thumbBin, thumbWidth, thumbHeight, srcImg.Width, srcImg.Height, fileUpdateDate);
                    DatabaseManager<ThumbnailConnection>.Update(sql);

                    ThumbnailBufferEntity thumb = new ThumbnailBufferEntity();
                    thumb.FilePath = filePath;
                    thumb.ThumbnailBuffer = thumbBin;
                    thumb.ThumbnailWidth = thumbWidth;
                    thumb.ThumbnailHeight = thumbHeight;
                    thumb.SourceWidth = srcImg.Width;
                    thumb.SourceHeight = srcImg.Height;
                    thumb.FileUpdatedate = fileUpdateDate;

                    return thumb;
                }
            }
        }

        private ThumbnailBufferEntity createDBDirectoryCash(string directoryPath, string thumbFilePath, int thumbWidth, int thumbHeight, DateTime directoryUpdateDate)
        {
            using (Image srcImg = ImageUtil.ReadImageFile(thumbFilePath))
            {
                using (Image thumbImg = ThumbnailUtil.CreateThumbnail(srcImg, thumbWidth, thumbHeight))
                {
                    byte[] thumbBin = ImageUtil.ToCompressionBinary(thumbImg);

                    CreationThumbnailSql sql = new CreationThumbnailSql(directoryPath, thumbBin, thumbWidth, thumbHeight, srcImg.Width, srcImg.Height, directoryUpdateDate);
                    DatabaseManager<ThumbnailConnection>.Update(sql);

                    ThumbnailBufferEntity thumb = new ThumbnailBufferEntity();
                    thumb.FilePath = directoryPath;
                    thumb.ThumbnailBuffer = thumbBin;
                    thumb.ThumbnailWidth = thumbWidth;
                    thumb.ThumbnailHeight = thumbHeight;
                    thumb.SourceWidth = srcImg.Width;
                    thumb.SourceHeight = srcImg.Height;
                    thumb.FileUpdatedate = directoryUpdateDate;
                    return thumb;
                }
            }
        }

        private ThumbnailBufferEntity updateDBDirectoryCash(string directoryPath, string thumbFilePath, int thumbWidth, int thumbHeight, DateTime directoryUpdateDate)
        {
            using (Image srcImg = ImageUtil.ReadImageFile(thumbFilePath))
            {
                using (Image thumbImg = ThumbnailUtil.CreateThumbnail(srcImg, thumbWidth, thumbHeight))
                {
                    byte[] thumbBin = ImageUtil.ToCompressionBinary(thumbImg);

                    UpdateThumbnailSql sql = new UpdateThumbnailSql(directoryPath, thumbBin, thumbWidth, thumbHeight, srcImg.Width, srcImg.Height, directoryUpdateDate);
                    DatabaseManager<ThumbnailConnection>.Update(sql);

                    ThumbnailBufferEntity thumb = new ThumbnailBufferEntity();
                    thumb.FilePath = directoryPath;
                    thumb.ThumbnailBuffer = thumbBin;
                    thumb.ThumbnailWidth = thumbWidth;
                    thumb.ThumbnailHeight = thumbHeight;
                    thumb.SourceWidth = srcImg.Width;
                    thumb.SourceHeight = srcImg.Height;
                    thumb.FileUpdatedate = directoryUpdateDate;

                    return thumb;
                }
            }
        }

        #endregion

        #region メモリーキャッシュ操作メソッド

        private static ThumbnailBufferEntity getMemoryCash(string filePath)
        {
            _cashLock.EnterReadLock();

            try
            {
                ThumbnailBufferEntity cach = null;
                if (_cashDic.TryGetValue(filePath, out cach))
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
                _cashLock.ExitReadLock();
            }
        }

        private static void updateMemoryCash(ThumbnailBufferEntity thumb)
        {
            _cashLock.EnterWriteLock();

            try
            {
                ThumbnailBufferEntity dicCash = null;
                if (_cashDic.TryGetValue(thumb.FilePath, out dicCash))
                {
                    _cashList.Remove(dicCash);
                    _cashList.Add(thumb);
                    _cashDic[thumb.FilePath] = thumb;
                }
                else
                {
                    if (_cashList.Count == _cashList.Capacity)
                    {
                        ThumbnailBufferEntity cash = _cashList[0];
                        _cashList.Remove(cash);
                        _cashDic.Remove(thumb.FilePath);
                    }

                    _cashList.Add(thumb);
                    _cashDic.Add(thumb.FilePath, thumb);
                }
            }
            finally
            {
                _cashLock.ExitWriteLock();
            }
        }

        #endregion
    }
}
