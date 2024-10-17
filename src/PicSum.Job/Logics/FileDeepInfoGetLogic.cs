using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using PicSum.Job.Entities;
using SWF.Core.DatabaseAccessor;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Drawing;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイルの深い情報取得ロジック
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class FileDeepInfoGetLogic
        : AbstractAsyncLogic
    {
        public FileDeepInfoGetLogic(AbstractAsyncJob job)
            : base(job)
        {

        }

        public FileDeepInfoEntity Execute(string filePath, Size thumbSize)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var info = new FileDeepInfoEntity
            {
                FilePath = filePath,
                FileName = FileUtil.GetFileName(filePath),
                FileType = FileUtil.GetTypeName(filePath)
            };

            if (FileUtil.IsSystemRoot(filePath))
            {
                info.UpdateDate = DateTime.MinValue;
                info.IsFile = false;
                info.IsImageFile = false;
                info.FileSize = null;
                info.FileIcon = FileIconCash.LargeMyComputerIcon;
            }
            else
            {
                info.UpdateDate = FileUtil.GetUpdateDate(filePath);
                info.IsFile = FileUtil.IsFile(filePath);
                if (info.IsFile)
                {
                    info.IsImageFile = FileUtil.IsImageFile(filePath);
                    info.FileSize = FileUtil.GetFileSize(filePath);
                    info.FileIcon = FileIconCash.GetLargeFileIcon(filePath);
                }
                else
                {
                    info.IsImageFile = false;
                    info.FileSize = null;
                    if (FileUtil.IsDrive(filePath))
                    {
                        info.FileIcon = FileIconCash.GetLargeDriveIcon(filePath);
                    }
                    else if (FileUtil.IsDirectory(filePath))
                    {
                        info.FileIcon = FileIconCash.JumboDirectoryIcon;
                    }
                    else
                    {
                        info.FileIcon = null;
                    }
                }

                if (info.IsImageFile)
                {
                    var srcSize = this.GetImageSize(filePath);
                    if (srcSize != ImageUtil.EMPTY_SIZE)
                    {
                        using (var tran = DatabaseManager<ThumbnailConnection>.BeginTransaction())
                        {
                            var thumbnailGetLogic = new ThumbnailGetLogic(this.Job);
                            var thumbnailBuffer
                                = thumbnailGetLogic.GetOrCreateCache(filePath, thumbSize.Width, thumbSize.Height);
                            var thumbnailImage
                                = ImageUtil.ToImage(thumbnailBuffer.ThumbnailBuffer);

                            info.Thumbnail = new()
                            {
                                FilePath = info.FilePath,
                                FileUpdatedate = info.UpdateDate,
                                ThumbnailImage = thumbnailImage,
                                ThumbnailWidth = thumbSize.Width,
                                ThumbnailHeight = thumbSize.Height,
                                SourceWidth = srcSize.Width,
                                SourceHeight = srcSize.Height
                            };
                            info.ImageSize = new(srcSize.Width, srcSize.Height);

                            tran.Commit();
                        }

                    }
                }
                else if (!info.IsFile)
                {
                    var firstImageFile = FileUtil.GetFirstImageFilePath(filePath);
                    if (!string.IsNullOrEmpty(firstImageFile))
                    {
                        var srcSize = this.GetImageSize(firstImageFile);
                        if (srcSize != ImageUtil.EMPTY_SIZE)
                        {
                            using (var tran = DatabaseManager<ThumbnailConnection>.BeginTransaction())
                            {
                                var thumbnailGetLogic = new ThumbnailGetLogic(this.Job);
                                var thumbnailBuffer
                                    = thumbnailGetLogic.GetOrCreateCache(firstImageFile, thumbSize.Width, thumbSize.Height);
                                var thumbnailImage
                                    = ImageUtil.ToImage(thumbnailBuffer.ThumbnailBuffer);

                                info.Thumbnail = new()
                                {
                                    FilePath = info.FilePath,
                                    FileUpdatedate = info.UpdateDate,
                                    ThumbnailImage = thumbnailImage,
                                    ThumbnailWidth = thumbSize.Width,
                                    ThumbnailHeight = thumbSize.Height,
                                    SourceWidth = srcSize.Width,
                                    SourceHeight = srcSize.Height,
                                };
                                info.ImageSize = new(srcSize.Width, srcSize.Height);

                                tran.Commit();
                            }
                        }
                    }
                }
            }

            this.CheckCancel();

            var sql = new FileInfoReadSql(info.FilePath);
            var dto = DatabaseManager<FileInfoConnection>.ReadLine<FileInfoDto>(sql);
            if (!dto.Equals(default(FileInfoDto)))
            {
                info.Rating = dto.Rating;
            }
            else
            {
                info.Rating = 0;
            }

            this.CheckCancel();

            return info;
        }

        private Size GetImageSize(string filePath)
        {
            try
            {
                return ImageFileSizeCacheUtil.Get(filePath).Size;
            }
            catch (FileUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.ID, ex));
                return ImageUtil.EMPTY_SIZE;
            }
            catch (ImageUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.ID, ex));
                return ImageUtil.EMPTY_SIZE;
            }
        }
    }
}
