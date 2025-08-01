using PicSum.Job.Common;
using PicSum.Job.Entities;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;
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
        public FileShallowInfoEntity Get(
            string filePath, bool isGetThumbnail, DateTime registrationDate)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (FileUtil.IsExistsFile(filePath))
            {
                return this.GetFileInfo(
                    filePath, isGetThumbnail, registrationDate);
            }
            else if (FileUtil.IsSystemRoot(filePath))
            {
                return this.GetSystemRootInfo(
                    filePath, registrationDate);
            }
            else if (FileUtil.IsExistsDrive(filePath))
            {
                return this.GetDriveInfo(
                    filePath, registrationDate);
            }
            else if (FileUtil.IsExistsDirectory(filePath))
            {
                return this.GetDirectoryInfo(
                    filePath, isGetThumbnail, registrationDate);
            }
            else
            {
                return FileShallowInfoEntity.EMPTY;
            }
        }

        public FileShallowInfoEntity Get(
            string filePath, bool isGetThumbnail)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return this.Get(filePath, isGetThumbnail, FileUtil.EMPTY_DATETIME);
        }

        private FileShallowInfoEntity GetFileInfo(
            string filePath, bool isGetThumbnail, DateTime registrationDate)
        {
            var (createDate, updateDate, _) = FileUtil.GetFileInfo(filePath);

            var info = new FileShallowInfoEntity
            {
                RgistrationDate = registrationDate,
                FilePath = filePath,
                FileName = FileUtil.GetFileName(filePath),
                IsFile = true,
                IsImageFile = ImageUtil.IsImageFile(filePath),
                CreateDate = createDate,
                UpdateDate = updateDate,
                SmallIcon = Instance<IFileIconCacher>.Value.GetSmallFileIcon(filePath),
                ExtraLargeIcon = Instance<IFileIconCacher>.Value.GetExtraLargeFileIcon(filePath),
                JumboIcon = Instance<IFileIconCacher>.Value.GetJumboFileIcon(filePath)
            };

            if (!isGetThumbnail)
            {
                return info;
            }

            var thumbnailBuffer = Instance<IThumbnailCacher>.Value.GetCache(filePath);
            if (thumbnailBuffer != ThumbnailCacheEntity.EMPTY
                && thumbnailBuffer.ThumbnailBuffer != null)
            {
                info.ThumbnailImage = new CvImage(
                    filePath,
                    ThumbnailUtil.ToImage(thumbnailBuffer.ThumbnailBuffer));
                info.ThumbnailWidth = thumbnailBuffer.ThumbnailWidth;
                info.ThumbnailHeight = thumbnailBuffer.ThumbnailHeight;
                info.SourceWidth = thumbnailBuffer.SourceWidth;
                info.SourceHeight = thumbnailBuffer.SourceHeight;

                return info;
            }
            else
            {
                return info;
            }
        }

        private FileShallowInfoEntity GetDirectoryInfo(
            string filePath, bool isGetThumbnail, DateTime registrationDate)
        {
            var (createDate, updateDate) = FileUtil.GetDirectoryInfo(filePath);

            var info = new FileShallowInfoEntity
            {
                RgistrationDate = registrationDate,
                FilePath = filePath,
                FileName = FileUtil.GetFileName(filePath),
                IsFile = false,
                IsImageFile = false,
                CreateDate = createDate,
                UpdateDate = updateDate,
                SmallIcon = Instance<IFileIconCacher>.Value.SmallDirectoryIcon,
                ExtraLargeIcon = Instance<IFileIconCacher>.Value.ExtraLargeDirectoryIcon,
                JumboIcon = Instance<IFileIconCacher>.Value.JumboDirectoryIcon
            };

            if (!isGetThumbnail)
            {
                return info;
            }

            var thumbnailBuffer = Instance<IThumbnailCacher>.Value.GetCache(filePath);
            if (thumbnailBuffer != ThumbnailCacheEntity.EMPTY
                && thumbnailBuffer.ThumbnailBuffer != null)
            {
                info.ThumbnailImage = new CvImage(
                    filePath,
                    ThumbnailUtil.ToImage(thumbnailBuffer.ThumbnailBuffer));
                info.ThumbnailWidth = thumbnailBuffer.ThumbnailWidth;
                info.ThumbnailHeight = thumbnailBuffer.ThumbnailHeight;
                info.SourceWidth = thumbnailBuffer.SourceWidth;
                info.SourceHeight = thumbnailBuffer.SourceHeight;
                return info;
            }
            else
            {
                return info;
            }
        }

        private FileShallowInfoEntity GetDriveInfo(
            string filePath, DateTime registrationDate)
        {
            var (createDate, updateDate) = FileUtil.GetDirectoryInfo(filePath);

            return new FileShallowInfoEntity
            {
                RgistrationDate = registrationDate,
                FilePath = filePath,
                FileName = FileUtil.GetFileName(filePath),
                IsFile = false,
                IsImageFile = false,
                CreateDate = createDate,
                UpdateDate = updateDate,
                SmallIcon = Instance<IFileIconCacher>.Value.GetSmallDriveIcon(filePath),
                ExtraLargeIcon = Instance<IFileIconCacher>.Value.GetExtraLargeDriveIcon(filePath),
                JumboIcon = Instance<IFileIconCacher>.Value.GetJumboDriveIcon(filePath)
            };
        }

        private FileShallowInfoEntity GetSystemRootInfo(
            string filePath, DateTime registrationDate)
        {
            return new FileShallowInfoEntity
            {
                RgistrationDate = registrationDate,
                FilePath = filePath,
                FileName = FileUtil.GetFileName(filePath),
                IsFile = false,
                IsImageFile = false,
                CreateDate = FileUtil.ROOT_DIRECTORY_DATETIME,
                UpdateDate = FileUtil.ROOT_DIRECTORY_DATETIME,
                SmallIcon = Instance<IFileIconCacher>.Value.SmallPCIcon,
                ExtraLargeIcon = Instance<IFileIconCacher>.Value.LargePCIcon,
                JumboIcon = Instance<IFileIconCacher>.Value.LargePCIcon
            };
        }
    }
}
