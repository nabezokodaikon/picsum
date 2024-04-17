using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Job.AsyncJob;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using PicSum.Job.Entities;
using SWF.Common;
using System;
using System.Drawing;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイルの深い情報取得ロジック
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class FileDeepInfoGetLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public FileDeepInfoEntity Execute(string filePath, Size thumbSize)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

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
                    var isImgSuccess = this.ReadImageFile(filePath, out var srcImg);
                    if (isImgSuccess)
                    {
                        using (srcImg)
                        {
                            var thumb = ThumbnailUtil.CreateThumbnail(srcImg, thumbSize.Width, thumbSize.Height);
                            info.Thumbnail = new()
                            {
                                FilePath = info.FilePath,
                                FileUpdatedate = info.UpdateDate,
                                ThumbnailImage = thumb,
                                ThumbnailWidth = thumbSize.Width,
                                ThumbnailHeight = thumbSize.Height,
                                SourceWidth = srcImg.Width,
                                SourceHeight = srcImg.Height
                            };
                            info.ImageSize = new(srcImg.Width, srcImg.Height);
                        }
                    }
                }
                else if (!info.IsFile)
                {
                    var firstImageFile = FileUtil.GetFirstImageFilePath(filePath);
                    if (!string.IsNullOrEmpty(firstImageFile))
                    {
                        var isImgSuccess = this.ReadImageFile(firstImageFile, out var srcImg);
                        if (isImgSuccess)
                        {
                            using (srcImg)
                            {
                                var thumb = ThumbnailUtil.CreateThumbnail(srcImg, thumbSize.Width, thumbSize.Height);
                                info.Thumbnail = new()
                                {
                                    FilePath = info.FilePath,
                                    FileUpdatedate = info.UpdateDate,
                                    ThumbnailImage = thumb,
                                    ThumbnailWidth = thumbSize.Width,
                                    ThumbnailHeight = thumbSize.Height,
                                    SourceWidth = srcImg.Width,
                                    SourceHeight = srcImg.Height,
                                };
                                info.ImageSize = new(srcImg.Width, srcImg.Height);
                            }
                        }
                    }
                }
            }

            this.CheckCancel();

            var sql = new FileInfoReadSql(info.FilePath);
            var dto = DatabaseManager<FileInfoConnection>.ReadLine<FileInfoDto>(sql);
            if (dto != null)
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

        private bool ReadImageFile(string filePath, out Bitmap? bmp)
        {
            try
            {
                bmp = ImageUtil.ReadImageFile(filePath);
                return true;
            }
            catch (ImageUtilException ex)
            {
                bmp = null;
                this.WriteErrorLog(new JobException(this.ID, ex));
                return false;
            }
        }
    }
}
