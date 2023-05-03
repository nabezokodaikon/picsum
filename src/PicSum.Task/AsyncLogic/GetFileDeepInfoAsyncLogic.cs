using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using PicSum.Task.Entity;
using SWF.Common;
using System;
using System.Drawing;
using System.IO;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// ファイルの深い情報取得ロジック
    /// </summary>
    internal sealed class GetFileDeepInfoAsyncLogic
        : AbstractAsyncLogic
    {
        public GetFileDeepInfoAsyncLogic(AbstractAsyncFacade facade)
            : base(facade)
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
            if (string.IsNullOrEmpty(filePath))
            {
                info.UpdateDate = null;
                info.CreateDate = null;
                info.IsFile = false;
                info.IsImageFile = false;
                info.FileSize = null;
                info.FileIcon = FileIconCash.LargeMyComputerIcon;
            }
            else
            {
                if (!FileUtil.CanAccess(filePath))
                {
                    throw new FileNotFoundException(string.Format("ファイル '{0}' が見つかりませんでした。", filePath));
                }

                info.UpdateDate = FileUtil.GetUpdateDate(filePath);
                info.CreateDate = FileUtil.GetCreateDate(filePath);

                info.IsFile = FileUtil.IsFile(filePath);

                if (info.IsFile)
                {
                    info.IsImageFile = FileUtil.IsImageFile(filePath);
                }
                else
                {
                    info.IsImageFile = false;
                }

                if (info.IsFile)
                {
                    info.FileSize = FileUtil.GetFileSize(filePath);
                    info.FileIcon = FileIconCash.GetJumboFileIcon(filePath);
                }
                else
                {
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
                    info.Thumbnail = new ThumbnailImageEntity();
                    using (var srcImg = ImageUtil.ReadImageFile(filePath))
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
                else if (!info.IsFile)
                {
                    var firstImageFile = FileUtil.GetFirstImageFilePath(filePath);
                    if (!string.IsNullOrEmpty(firstImageFile))
                    {
                        info.Thumbnail = new ThumbnailImageEntity();
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
