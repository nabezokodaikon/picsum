using PicSum.Job.Common;
using PicSum.Job.Entities;
using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイルの浅い情報を取得します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class FileShallowInfoGetLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        private const int THUMBNAIL_SIZE = 248;

        public FileShallowInfoEntity Execute(string filePath, bool isGetThumbnail)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var info = new FileShallowInfoEntity
            {
                RgistrationDate = FileUtil.EMPTY_DATETIME,
            };

            if (FileUtil.IsSystemRoot(filePath))
            {
                info.FilePath = filePath;
                info.FileName = FileUtil.GetFileName(filePath);
                info.IsFile = false;
                info.IsImageFile = false;
                info.UpdateDate = FileUtil.EMPTY_DATETIME;
                info.SmallIcon = Instance<IFileIconCacher>.Value.SmallPCIcon;
                info.ExtraLargeIcon = Instance<IFileIconCacher>.Value.LargePCIcon;
                info.JumboIcon = Instance<IFileIconCacher>.Value.LargePCIcon;
            }
            else if (FileUtil.IsExistsDrive(filePath))
            {
                info.FilePath = filePath;
                info.FileName = FileUtil.GetFileName(filePath);
                info.IsFile = false;
                info.IsImageFile = false;
                info.UpdateDate = FileUtil.GetUpdateDate(filePath);
                info.SmallIcon = Instance<IFileIconCacher>.Value.GetSmallDriveIcon(info.FilePath);
                info.ExtraLargeIcon = Instance<IFileIconCacher>.Value.GetExtraLargeDriveIcon(info.FilePath);
                info.JumboIcon = Instance<IFileIconCacher>.Value.GetJumboDriveIcon(info.FilePath);
            }
            else if (FileUtil.IsExistsFile(filePath))
            {
                info.FilePath = filePath;
                info.FileName = FileUtil.GetFileName(filePath);
                info.IsFile = true;
                info.IsImageFile = FileUtil.IsImageFile(filePath);
                info.UpdateDate = FileUtil.GetUpdateDate(filePath);
                info.SmallIcon = Instance<IFileIconCacher>.Value.GetSmallFileIcon(info.FilePath);
                info.ExtraLargeIcon = Instance<IFileIconCacher>.Value.GetExtraLargeFileIcon(info.FilePath);
                info.JumboIcon = Instance<IFileIconCacher>.Value.GetJumboFileIcon(info.FilePath);

                if (isGetThumbnail)
                {
                    var thumbnailBuffer = Instance<IThumbnailCacher>.Value.GetOnlyCache(filePath, THUMBNAIL_SIZE, THUMBNAIL_SIZE);
                    if (thumbnailBuffer != ThumbnailCacheEntity.EMPTY)
                    {
                        if (thumbnailBuffer.ThumbnailBuffer == null)
                        {
                            throw new NullReferenceException("サムネイルのバッファがNullです。");
                        }

                        info.ThumbnailImage = ThumbnailUtil.ToImage(thumbnailBuffer.ThumbnailBuffer);
                        info.ThumbnailWidth = THUMBNAIL_SIZE;
                        info.ThumbnailHeight = THUMBNAIL_SIZE;
                        info.SourceWidth = thumbnailBuffer.SourceWidth;
                        info.SourceHeight = thumbnailBuffer.SourceHeight;
                    }
                }
            }
            else if (FileUtil.IsExistsDirectory(filePath))
            {
                info.FilePath = filePath;
                info.FileName = FileUtil.GetFileName(filePath);
                info.IsFile = false;
                info.IsImageFile = false;
                info.UpdateDate = FileUtil.GetUpdateDate(filePath);
                info.SmallIcon = Instance<IFileIconCacher>.Value.SmallDirectoryIcon;
                info.ExtraLargeIcon = Instance<IFileIconCacher>.Value.ExtraLargeDirectoryIcon;
                info.JumboIcon = Instance<IFileIconCacher>.Value.JumboDirectoryIcon;

                if (isGetThumbnail)
                {
                    var thumbnailBuffer = Instance<IThumbnailCacher>.Value.GetOnlyCache(filePath, THUMBNAIL_SIZE, THUMBNAIL_SIZE);
                    if (thumbnailBuffer != ThumbnailCacheEntity.EMPTY)
                    {
                        if (thumbnailBuffer.ThumbnailBuffer == null)
                        {
                            throw new NullReferenceException("サムネイルのバッファがNullです。");
                        }

                        info.ThumbnailImage = ThumbnailUtil.ToImage(thumbnailBuffer.ThumbnailBuffer);
                        info.ThumbnailWidth = THUMBNAIL_SIZE;
                        info.ThumbnailHeight = THUMBNAIL_SIZE;
                        info.SourceWidth = thumbnailBuffer.SourceWidth;
                        info.SourceHeight = thumbnailBuffer.SourceHeight;
                    }
                }
            }
            else
            {
                throw new ArgumentException($"不正なファイルパスです。'{filePath}'", nameof(filePath));
            }

            return info;
        }

        public FileShallowInfoEntity Execute(
            string filePath, DateTime registrationDate, bool isGetThumbnail)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var info = this.Execute(filePath, isGetThumbnail);
            info.RgistrationDate = registrationDate;

            return info;
        }
    }
}
