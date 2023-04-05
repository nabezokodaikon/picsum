using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Data.FileAccessor;
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
        private readonly IList<string> _imageFileExtensionList = ImageUtil.ImageFileExtensionList;

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
                string ex = FileUtil.GetExtension(filePath);
                if (!_imageFileExtensionList.Contains(ex))
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
                ThumbnailBufferEntity cash = getFolderCash(filePath, thumbWidth, thumbHeight);
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
                DateTime updateDate = FileUtil.GetUpdateDate(filePath);
                if (memCash.ThumbnailWidth >= thumbWidth &&
                    memCash.ThumbnailHeight >= thumbHeight &&
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
                            dbCash.FileUpdatedate >= updateDate)
                        {
                            // DBキャッシュを返します。
                            updateMemoryCash(dbCash);
                            return dbCash;
                        }
                        else
                        {
                            // サムネイルを更新します。
                            ThumbnailBufferEntity thumb = updateDBFileCash(filePath, thumbWidth, thumbHeight, updateDate);
                            updateMemoryCash(thumb);
                            return thumb;
                        }
                    }
                    else
                    {
                        // サムネイルを作成します。
                        ThumbnailBufferEntity thumb = createDBFileCash(filePath, thumbWidth, thumbHeight, updateDate);
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
                    DateTime updateDate = FileUtil.GetUpdateDate(filePath);
                    if (dbCash.ThumbnailWidth >= thumbWidth &&
                        dbCash.ThumbnailHeight >= thumbHeight &&
                        dbCash.FileUpdatedate >= updateDate)
                    {
                        // DBキャッシュを返します。
                        updateMemoryCash(dbCash);
                        return dbCash;
                    }
                    else
                    {
                        // サムネイルを更新します。
                        ThumbnailBufferEntity thumb = updateDBFileCash(filePath, thumbWidth, thumbHeight, updateDate);
                        updateMemoryCash(thumb);
                        return thumb;
                    }
                }
                else
                {
                    // サムネイルを作成します。
                    DateTime updateDate = FileUtil.GetUpdateDate(filePath);
                    ThumbnailBufferEntity thumb = createDBFileCash(filePath, thumbWidth, thumbHeight, updateDate);
                    updateMemoryCash(thumb);
                    return thumb;
                }
            }
        }

        private ThumbnailBufferEntity getFolderCash(string filePath, int thumbWidth, int thumbHeight)
        {
            ThumbnailBufferEntity memCash = getMemoryCash(filePath);
            if (memCash != null)
            {
                DateTime updateDate = FileUtil.GetUpdateDate(filePath);
                if (memCash.ThumbnailWidth >= thumbWidth &&
                    memCash.ThumbnailHeight >= thumbHeight &&
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
                            dbCash.FileUpdatedate >= updateDate)
                        {
                            // DBキャッシュを返します。
                            updateMemoryCash(dbCash);
                            return dbCash;
                        }
                        else
                        {
                            string thumbFile = getFirstImageFilePath(filePath);
                            if (!string.IsNullOrEmpty(thumbFile))
                            {
                                // サムネイルを更新します。
                                ThumbnailBufferEntity thumb = updateDBFolderCash(filePath, thumbFile, thumbWidth, thumbHeight, updateDate);
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
                        string thumbFile = getFirstImageFilePath(filePath);
                        if (!string.IsNullOrEmpty(thumbFile))
                        {
                            // サムネイルを作成します。
                            ThumbnailBufferEntity thumb = createDBFolderCash(filePath, thumbFile, thumbWidth, thumbHeight, updateDate);
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
                    DateTime updateDate = FileUtil.GetUpdateDate(filePath);
                    if (dbCash.ThumbnailWidth >= thumbWidth &&
                        dbCash.ThumbnailHeight >= thumbHeight &&
                        dbCash.FileUpdatedate >= updateDate)
                    {
                        // DBキャッシュを返します。
                        updateMemoryCash(dbCash);
                        return dbCash;
                    }
                    else
                    {
                        string thumbFile = getFirstImageFilePath(filePath);
                        if (!string.IsNullOrEmpty(thumbFile))
                        {
                            // サムネイルを更新します。                                
                            ThumbnailBufferEntity thumb = updateDBFolderCash(filePath, thumbFile, thumbWidth, thumbHeight, updateDate);
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
                    string thumbFile = getFirstImageFilePath(filePath);
                    if (!string.IsNullOrEmpty(thumbFile))
                    {
                        DateTime updateDate = FileUtil.GetUpdateDate(filePath);
                        ThumbnailBufferEntity thumb = createDBFolderCash(filePath, thumbFile, thumbWidth, thumbHeight, updateDate);
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

        private string getFirstImageFilePath(string folderPath)
        {
            return FileUtil.GetFiles(folderPath).OrderBy(file => file).FirstOrDefault(file => ImageUtil.ImageFileExtensionList.Contains(FileUtil.GetExtension(file)) &&
                                                                        FileUtil.CanAccess(file));
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

        private ThumbnailBufferEntity createDBFolderCash(string folderPath, string thumbFilePath, int thumbWidth, int thumbHeight, DateTime folderUpdateDate)
        {
            using (Image srcImg = ImageUtil.ReadImageFile(thumbFilePath))
            {
                using (Image thumbImg = ThumbnailUtil.CreateThumbnail(srcImg, thumbWidth, thumbHeight))
                {
                    byte[] thumbBin = ImageUtil.ToCompressionBinary(thumbImg);

                    CreationThumbnailSql sql = new CreationThumbnailSql(folderPath, thumbBin, thumbWidth, thumbHeight, srcImg.Width, srcImg.Height, folderUpdateDate);
                    DatabaseManager<ThumbnailConnection>.Update(sql);

                    ThumbnailBufferEntity thumb = new ThumbnailBufferEntity();
                    thumb.FilePath = folderPath;
                    thumb.ThumbnailBuffer = thumbBin;
                    thumb.ThumbnailWidth = thumbWidth;
                    thumb.ThumbnailHeight = thumbHeight;
                    thumb.SourceWidth = srcImg.Width;
                    thumb.SourceHeight = srcImg.Height;
                    thumb.FileUpdatedate = folderUpdateDate;
                    return thumb;
                }
            }
        }

        private ThumbnailBufferEntity updateDBFolderCash(string folderPath, string thumbFilePath, int thumbWidth, int thumbHeight, DateTime folderUpdateDate)
        {
            using (Image srcImg = ImageUtil.ReadImageFile(thumbFilePath))
            {
                using (Image thumbImg = ThumbnailUtil.CreateThumbnail(srcImg, thumbWidth, thumbHeight))
                {
                    byte[] thumbBin = ImageUtil.ToCompressionBinary(thumbImg);

                    UpdateThumbnailSql sql = new UpdateThumbnailSql(folderPath, thumbBin, thumbWidth, thumbHeight, srcImg.Width, srcImg.Height, folderUpdateDate);
                    DatabaseManager<ThumbnailConnection>.Update(sql);

                    ThumbnailBufferEntity thumb = new ThumbnailBufferEntity();
                    thumb.FilePath = folderPath;
                    thumb.ThumbnailBuffer = thumbBin;
                    thumb.ThumbnailWidth = thumbWidth;
                    thumb.ThumbnailHeight = thumbHeight;
                    thumb.SourceWidth = srcImg.Width;
                    thumb.SourceHeight = srcImg.Height;
                    thumb.FileUpdatedate = folderUpdateDate;

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
                if (_cashDic.ContainsKey(filePath))
                {
                    return _cashDic[filePath];
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
                if (_cashDic.ContainsKey(thumb.FilePath))
                {
                    ThumbnailBufferEntity cash = _cashDic[thumb.FilePath];
                    _cashList.Remove(cash);
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
