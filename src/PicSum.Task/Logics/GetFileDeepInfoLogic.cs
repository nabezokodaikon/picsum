using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using PicSum.Task.Entities;
using PicSum.Task.Results;
using SWF.Common;
using System;
using System.Drawing;
using System.Runtime.Versioning;

namespace PicSum.Task.Logics
{
    /// <summary>
    /// ファイルの深い情報取得ロジック
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class GetFileDeepInfoLogic
        : AbstractAsyncLogic
    {
        public GetFileDeepInfoLogic(IAsyncTask task)
            : base(task)
        {

        }

        public FileDeepInfoEntity Execute(string filePath, Size thumbSize)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var info = new FileDeepInfoEntity();
            info.FilePath = filePath;
            info.FileName = FileUtil.GetFileName(filePath);
            info.FileType = FileUtil.GetTypeName(filePath);
            if (FileUtil.IsSystemRoot(filePath))
            {
                info.UpdateDate = null;
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
                    info.FileIcon = FileIconCash.GetJumboFileIcon(filePath);
                }
                else
                {
                    info.IsImageFile = false;
                    info.FileSize = null;
                    if (FileUtil.IsDrive(filePath))
                    {
                        info.FileIcon = FileIconCash.GetLargeDriveIcon(filePath);
                    }
                    else
                    {
                        info.FileIcon = FileIconCash.JumboDirectoryIcon;
                    }
                }

                if (info.IsImageFile)
                {
                    info.Thumbnail = new ThumbnailImageResult();
                    using (var srcImg = ImageUtil.ReadImageFile(filePath))
                    {
                        var thumb = ThumbnailUtil.CreateThumbnail(srcImg, thumbSize.Width, thumbSize.Height);
                        info.Thumbnail.FilePath = info.FilePath;
                        info.Thumbnail.FileUpdatedate = info.UpdateDate.Value;
                        info.Thumbnail.ThumbnailImage = thumb;
                        info.Thumbnail.ThumbnailWidth = thumbSize.Width;
                        info.Thumbnail.ThumbnailHeight = thumbSize.Height;
                        info.Thumbnail.SourceWidth = srcImg.Width;
                        info.Thumbnail.SourceHeight = srcImg.Height;
                        info.ImageSize = new Size(srcImg.Width, srcImg.Height);
                    }
                }
                else if (!info.IsFile)
                {
                    var firstImageFile = FileUtil.GetFirstImageFilePath(filePath);
                    if (!string.IsNullOrEmpty(firstImageFile))
                    {
                        info.Thumbnail = new ThumbnailImageResult();
                        using (var srcImg = ImageUtil.ReadImageFile(firstImageFile))
                        {
                            Image thumb = ThumbnailUtil.CreateThumbnail(srcImg, thumbSize.Width, thumbSize.Height);
                            info.Thumbnail.FilePath = info.FilePath;
                            info.Thumbnail.FileUpdatedate = info.UpdateDate.Value;
                            info.Thumbnail.ThumbnailImage = thumb;
                            info.Thumbnail.ThumbnailWidth = thumbSize.Width;
                            info.Thumbnail.ThumbnailHeight = thumbSize.Height;
                            info.Thumbnail.SourceWidth = srcImg.Width;
                            info.Thumbnail.SourceHeight = srcImg.Height;
                            info.ImageSize = new Size(srcImg.Width, srcImg.Height);
                        }
                    }
                }
            }

            this.CheckCancel();

            var sql = new ReadFileInfoSql(info.FilePath);
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
    }
}
