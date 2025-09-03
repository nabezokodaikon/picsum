using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using PicSum.Job.Entities;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;
using System.Drawing;

namespace PicSum.Job.Common
{
    /// <summary>
    /// サムネイルを読込みます。
    /// </summary>

    internal sealed partial class ThumbnailCacher
        : IThumbnailCacher
    {
        private const int CACHE_CAPACITY = 100 * 1024 * 1024;

        private bool _disposed = false;
        private CacheFileController? _cacheFileController = null;

        public async ValueTask Initialize()
        {
            await using (var con = await Instance<IThumbnailDao>.Value.Connect())
            {
                var position = (int)await con.ReadValue<long>(new ThumbnailIDReadSql());

                this._cacheFileController = new(
                    AppFiles.THUMBNAIL_CACHE_FILE.Value,
                    CACHE_CAPACITY,
                    position);
            }
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

        public async ValueTask<ThumbnailCacheEntity> GetCache(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (FileUtil.IsExistsFile(filePath) && ImageUtil.IsImageFile(filePath))
            {
                return await this.GetFileCache(filePath).WithConfig();
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
                return await this.GetDirectoryCache(filePath).WithConfig();
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

                var cache = await this.GetOrCreateFileCache(filePath, thumbWidth, thumbHeight).WithConfig();
                if (!cache.IsEmpry)
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
                var cache = await this.GetOrCreateDirectoryCache(filePath, thumbWidth, thumbHeight).WithConfig();
                if (!cache.IsEmpry)
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
            var cache = await this.GetDBCache(filePath).WithConfig();
            if (!cache.IsEmpry)
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
            return await this.GetFileCache(directoryPath).WithConfig();
        }

        private async ValueTask<ThumbnailCacheEntity> GetOrCreateFileCache(
            string filePath, int thumbWidth, int thumbHeight)
        {
            var dbCache = await this.GetDBCache(filePath).WithConfig();
            if (!dbCache.IsEmpry)
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
                        filePath, filePath, thumbWidth, thumbHeight, updateDate).WithConfig();
                    return thumb;
                }
            }
            else
            {
                // サムネイルを作成します。
                var updateDate = FileUtil.GetUpdateDate(filePath);
                var thumb = await this.CreateDBCache(
                    filePath, filePath, thumbWidth, thumbHeight, updateDate).WithConfig();
                return thumb;
            }
        }

        private async ValueTask<ThumbnailCacheEntity> GetOrCreateDirectoryCache(
            string directoryPath, int thumbWidth, int thumbHeight)
        {
            var dbCache = await this.GetDBCache(directoryPath).WithConfig();
            if (!dbCache.IsEmpry)
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
                        directoryPath, thumbFilePath, thumbWidth, thumbHeight, updateDate).WithConfig();

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
                    directoryPath, thumbFilePath, thumbWidth, thumbHeight, updateDate).WithConfig();

                return thumb;
            }
        }

        private async ValueTask<ThumbnailCacheEntity> GetDBCache(string filePath)
        {
            if (this._cacheFileController is null)
            {
                throw new InvalidOperationException("キャッシュファイルコントローラが初期化されていません。");
            }

            using (TimeMeasuring.Run(false, "ThumbnailCacher.GetDBCache"))
            {
                await using (var con = await Instance<IThumbnailDao>.Value.Connect().WithConfig())
                {
                    var sql = new ThumbnailReadByFileSql(filePath);
                    var dto = await con.ReadLine(sql).WithConfig();

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
            if (this._cacheFileController is null)
            {
                throw new InvalidOperationException("キャッシュファイルコントローラが初期化されていません。");
            }

            using (TimeMeasuring.Run(false, "ThumbnailCacher.CreateDBCache"))
            {
                using (var srcImg = await ImageUtil.ReadImageFile(thumbFilePath).WithConfig())
                {
                    Instance<IImageFileSizeCacher>.Value.Set(
                        thumbFilePath, new Size(srcImg.Width, srcImg.Height));

                    using (var thumbImg = ThumbnailUtil.CreateThumbnail(
                        srcImg, thumbWidth, thumbHeight))
                    {
                        var thumbBin = ThumbnailUtil.ToCompressionBinary(thumbImg);
                        await using (var con = await Instance<IThumbnailDao>.Value.Connect().WithConfig())
                        {
                            var exists = await con.ReadValue<long>(
                                new ThumbnailExistsByFileSql(targetFilePath)).WithConfig();
                            if (exists < 1)
                            {
                                var position = this._cacheFileController.Write(thumbBin);
                                await con.Update(new ThumbnailOffsetUpdateSql(position)).WithConfig();

                                var thumbStartPoint = position - thumbBin.Length;
                                if (thumbStartPoint < 1)
                                {
                                    await con.Update(new ThumbnailDBCleanupSql()).WithConfig();
                                    await con.ReadLine(new ThumbnailDBVacuumSql()).WithConfig();
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
                                await con.Update(sql).WithConfig();
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
            if (this._cacheFileController is null)
            {
                throw new InvalidOperationException("キャッシュファイルコントローラが初期化されていません。");
            }

            using (TimeMeasuring.Run(false, "ThumbnailCacher.UpdateDBCache"))
            {
                using (var srcImg = await ImageUtil.ReadImageFile(thumbFilePath).WithConfig())
                {
                    Instance<IImageFileSizeCacher>.Value.Set(
                        thumbFilePath, new Size(srcImg.Width, srcImg.Height));

                    using (var thumbImg = ThumbnailUtil.CreateThumbnail(
                        srcImg, thumbWidth, thumbHeight))
                    {
                        var thumbBin = ThumbnailUtil.ToCompressionBinary(thumbImg);
                        await using (var con = await Instance<IThumbnailDao>.Value.Connect().WithConfig())
                        {
                            var position = this._cacheFileController.Write(thumbBin);
                            await con.Update(new ThumbnailOffsetUpdateSql(position)).WithConfig();

                            var thumbStartPoint = position - thumbBin.Length;
                            if (thumbStartPoint < 1)
                            {
                                await con.Update(new ThumbnailDBCleanupSql()).WithConfig();
                                await con.ReadLine(new ThumbnailDBVacuumSql()).WithConfig();
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
                            await con.Update(sql).WithConfig();
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
