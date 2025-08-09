using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using PicSum.Job.Entities;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.ResourceAccessor;
using System.Drawing;
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
        private const int CACHE_CAPACITY = 100 * 1024 * 1024;

        private bool _disposed = false;
        private CacheFileController? _cacheFileController = null;

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
                this._cacheFileController?.Dispose();
            }

            this._disposed = true;
        }

        public async ValueTask Initialize()
        {
            await using (var con = await Instance<IThumbnailDB>.Value.Connect())
            {
                var position = (int)con.ReadValue<long>(new ThumbnailIDReadSql());

                if (this._cacheFileController != null)
                {
                    throw new InvalidOperationException("キャッシュファイルコントローラは既に初期化されています。");
                }

                this._cacheFileController = new(
                    AppFiles.THUMBNAIL_CACHE_FILE.Value,
                    CACHE_CAPACITY,
                    position);
            }
        }

        public async ValueTask<ThumbnailCacheEntity> GetCache(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (FileUtil.IsExistsFile(filePath) && ImageUtil.IsImageFile(filePath))
            {
                return await this.GetFileCache(filePath);
            }
            else if (FileUtil.IsSystemRoot(filePath))
            {
                return ThumbnailCacheEntity.EMPTY;
            }
            else if (FileUtil.IsExistsDrive(filePath))
            {
                return ThumbnailCacheEntity.EMPTY;
            }
            else if (FileUtil.IsExistsDirectory(filePath))
            {
                return await this.GetDirectoryCache(filePath);
            }
            else
            {
                return ThumbnailCacheEntity.EMPTY;
            }
        }

        public async ValueTask<ThumbnailCacheEntity> GetOrCreateCache(
            string filePath, int thumbWidth, int thumbHeight)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (FileUtil.IsExistsFile(filePath))
            {
                if (!ImageUtil.IsImageFile(filePath))
                {
                    return ThumbnailCacheEntity.EMPTY;
                }

                var cache = await this.GetOrCreateFileCache(filePath, thumbWidth, thumbHeight);
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
            else if (FileUtil.IsExistsDirectory(filePath))
            {
                var cache = await this.GetOrCreateDirectoryCache(filePath, thumbWidth, thumbHeight);
                if (cache != ThumbnailCacheEntity.EMPTY)
                {
                    return cache;
                }
                else
                {
                    return ThumbnailCacheEntity.EMPTY;
                }
            }
            else
            {
                return ThumbnailCacheEntity.EMPTY;
            }
        }

        private async ValueTask<ThumbnailCacheEntity> GetFileCache(string filePath)
        {
            var cache = await this.GetDBCache(filePath);
            if (cache != ThumbnailCacheEntity.EMPTY)
            {
                var updateDate = FileUtil.GetUpdateDate(filePath);
                if (cache.FileUpdateDate >= updateDate)
                {
                    return cache;
                }
            }

            return ThumbnailCacheEntity.EMPTY;
        }

        private async ValueTask<ThumbnailCacheEntity> GetDirectoryCache(string directoryPath)
        {
            return await this.GetFileCache(directoryPath);
        }

        private async ValueTask<ThumbnailCacheEntity> GetOrCreateFileCache(
            string filePath, int thumbWidth, int thumbHeight)
        {
            var dbCache = await this.GetDBCache(filePath);
            if (dbCache != ThumbnailCacheEntity.EMPTY)
            {
                var updateDate = FileUtil.GetUpdateDate(filePath);

                if (dbCache.ThumbnailWidth >= thumbWidth &&
                    dbCache.ThumbnailHeight >= thumbHeight &&
                    dbCache.FileUpdateDate >= updateDate)
                {
                    // DBキャッシュを返します。
                    return dbCache;
                }
                else
                {
                    // サムネイルを更新します。
                    var thumb = await this.UpdateDBCache(
                        filePath, filePath, thumbWidth, thumbHeight, updateDate);
                    return thumb;
                }
            }
            else
            {
                // サムネイルを作成します。
                var updateDate = FileUtil.GetUpdateDate(filePath);
                var thumb = await this.CreateDBCache(
                    filePath, filePath, thumbWidth, thumbHeight, updateDate);
                return thumb;
            }
        }

        private async ValueTask<ThumbnailCacheEntity> GetOrCreateDirectoryCache(
            string directoryPath, int thumbWidth, int thumbHeight)
        {
            var dbCache = await this.GetDBCache(directoryPath);
            if (dbCache != ThumbnailCacheEntity.EMPTY)
            {
                var updateDate = FileUtil.GetUpdateDate(directoryPath);

                if (dbCache.ThumbnailWidth >= thumbWidth &&
                    dbCache.ThumbnailHeight >= thumbHeight &&
                    dbCache.FileUpdateDate >= updateDate)
                {
                    // DBキャッシュを返します。
                    return dbCache;
                }
                else
                {
                    // サムネイルを更新します。
                    var thumbFilePath = ImageUtil.GetFirstImageFilePath(directoryPath);
                    if (string.IsNullOrEmpty(thumbFilePath))
                    {
                        return ThumbnailCacheEntity.EMPTY;
                    }

                    var thumb = await this.UpdateDBCache(
                        directoryPath, thumbFilePath, thumbWidth, thumbHeight, updateDate);

                    return thumb;
                }
            }
            else
            {
                // サムネイルを作成します。
                var thumbFilePath = ImageUtil.GetFirstImageFilePath(directoryPath);
                if (string.IsNullOrEmpty(thumbFilePath))
                {
                    return ThumbnailCacheEntity.EMPTY;
                }

                var updateDate = FileUtil.GetUpdateDate(directoryPath);
                var thumb = await this.CreateDBCache(
                    directoryPath, thumbFilePath, thumbWidth, thumbHeight, updateDate);

                return thumb;
            }
        }

        private async ValueTask<ThumbnailCacheEntity> GetDBCache(string filePath)
        {
            using (TimeMeasuring.Run(false, "ThumbnailCacher.GetDBCache"))
            {
                await using (var con = await Instance<IThumbnailDB>.Value.Connect())
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
                            FileUpdateDate = dto.FileUpdateDate
                        };

                        if (this._cacheFileController == null)
                        {
                            throw new InvalidOperationException("キャッシュファイルコントローラが初期化されていません。");
                        }

                        thumb.ThumbnailBuffer = this._cacheFileController.Read(
                            dto.ThumbnailStartPoint, dto.ThumbnailSize);

                        return thumb;
                    }
                    else
                    {
                        return ThumbnailCacheEntity.EMPTY;
                    }
                }
            }
        }

        private async ValueTask<ThumbnailCacheEntity> CreateDBCache(
            string targetFilePath,
            string thumbFilePath,
            int thumbWidth,
            int thumbHeight,
            DateTime updateDate)
        {
            using (TimeMeasuring.Run(false, "ThumbnailCacher.CreateDBCache"))
            {
                using (var srcImg = ImageUtil.ReadImageFile(thumbFilePath))
                {
                    Instance<IImageFileSizeCacher>.Value.Set(
                        thumbFilePath, new Size(srcImg.Width, srcImg.Height));

                    using (var thumbImg = ThumbnailUtil.CreateThumbnail(
                        srcImg, thumbWidth, thumbHeight))
                    {
                        var thumbBin = ThumbnailUtil.ToCompressionBinary(thumbImg);
                        await using (var con = await Instance<IThumbnailDB>.Value.Connect())
                        {
                            var exists = con.ReadValue<long>(
                                new ThumbnailExistsByFileSql(targetFilePath));
                            if (exists < 1)
                            {
                                if (this._cacheFileController == null)
                                {
                                    throw new InvalidOperationException("キャッシュファイルコントローラが初期化されていません。");
                                }

                                var position = this._cacheFileController.Write(thumbBin);
                                con.Update(new ThumbnailOffsetUpdateSql(position));

                                var thumbStartPoint = position - thumbBin.Length;
                                if (thumbStartPoint < 1)
                                {
                                    con.Update(new ThumbnailDBCleanupSql());
                                    con.ReadLine(new ThumbnailDBVacuumSql());
                                }

                                var sql = new ThumbnailCreationSql(
                                    targetFilePath,
                                    0,
                                    thumbStartPoint,
                                    thumbBin.Length,
                                    thumbWidth,
                                    thumbHeight,
                                    srcImg.Width,
                                    srcImg.Height,
                                    updateDate);
                                con.Update(sql);
                            }
                        }

                        var thumb = new ThumbnailCacheEntity
                        {
                            FilePath = targetFilePath,
                            ThumbnailBuffer = thumbBin,
                            ThumbnailWidth = thumbWidth,
                            ThumbnailHeight = thumbHeight,
                            SourceWidth = srcImg.Width,
                            SourceHeight = srcImg.Height,
                            FileUpdateDate = updateDate
                        };

                        return thumb;
                    }
                }
            }
        }

        private async ValueTask<ThumbnailCacheEntity> UpdateDBCache(
            string targetFilePath,
            string thumbFilePath,
            int thumbWidth,
            int thumbHeight,
            DateTime updateDate)
        {
            using (TimeMeasuring.Run(false, "ThumbnailCacher.UpdateDBCache"))
            {
                using (var srcImg = ImageUtil.ReadImageFile(thumbFilePath))
                {
                    Instance<IImageFileSizeCacher>.Value.Set(
                        thumbFilePath, new Size(srcImg.Width, srcImg.Height));

                    using (var thumbImg = ThumbnailUtil.CreateThumbnail(
                        srcImg, thumbWidth, thumbHeight))
                    {
                        var thumbBin = ThumbnailUtil.ToCompressionBinary(thumbImg);
                        await using (var con = await Instance<IThumbnailDB>.Value.Connect())
                        {
                            if (this._cacheFileController == null)
                            {
                                throw new InvalidOperationException("キャッシュファイルコントローラが初期化されていません。");
                            }

                            var position = this._cacheFileController.Write(thumbBin);
                            con.Update(new ThumbnailOffsetUpdateSql(position));

                            var thumbStartPoint = position - thumbBin.Length;
                            if (thumbStartPoint < 1)
                            {
                                con.Update(new ThumbnailDBCleanupSql());
                                con.ReadLine(new ThumbnailDBVacuumSql());
                            }

                            var sql = new ThumbnailUpdateSql(
                                targetFilePath,
                                0,
                                thumbStartPoint,
                                thumbBin.Length,
                                thumbWidth,
                                thumbHeight,
                                srcImg.Width,
                                srcImg.Height,
                                updateDate);
                            con.Update(sql);
                        }

                        var thumb = new ThumbnailCacheEntity
                        {
                            FilePath = targetFilePath,
                            ThumbnailBuffer = thumbBin,
                            ThumbnailWidth = thumbWidth,
                            ThumbnailHeight = thumbHeight,
                            SourceWidth = srcImg.Width,
                            SourceHeight = srcImg.Height,
                            FileUpdateDate = updateDate
                        };

                        return thumb;
                    }
                }
            }
        }
    }
}
