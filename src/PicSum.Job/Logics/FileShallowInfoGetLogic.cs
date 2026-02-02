using PicSum.Job.Common;
using PicSum.Job.Entities;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;
using System.Drawing;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイルの浅い情報を取得します。
    /// </summary>

    internal sealed class FileShallowInfoGetLogic(IJob job)
        : AbstractLogic(job)
    {
        public async ValueTask<FileShallowInfoEntity> Get(
            string filePath, bool isGetThumbnail, DateTime addDate)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (!FileUtil.CanAccess(filePath))
            {
                return FileShallowInfoEntity.EMPTY;
            }

            if (FileUtil.IsExistsFile(filePath))
            {
                return await this.GetFileInfo(
                    filePath, isGetThumbnail, addDate).False();
            }
            else if (FileUtil.IsSystemRoot(filePath))
            {
                return this.GetSystemRootInfo(
                    filePath, addDate);
            }
            else if (FileUtil.IsExistsDrive(filePath))
            {
                return this.GetDriveInfo(
                    filePath, addDate);
            }
            else if (FileUtil.IsExistsDirectory(filePath))
            {
                return await this.GetDirectoryInfo(
                    filePath, isGetThumbnail, addDate).False();
            }
            else
            {
                return FileShallowInfoEntity.EMPTY;
            }
        }

        public async ValueTask<FileShallowInfoEntity> Get(
            string filePath, bool isGetThumbnail)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return await this.Get(filePath, isGetThumbnail, DateTimeExtensions.EMPTY).False();
        }

        private async ValueTask<FileShallowInfoEntity> GetFileInfo(
            string filePath, bool isGetThumbnail, DateTime addDate)
        {
            var (createDate, updateDate, _) = FileUtil.GetFileInfo(filePath);

            var info = new FileShallowInfoEntity
            {
                RgistrationDate = addDate,
                FilePath = filePath,
                FileName = FileUtil.GetFileName(filePath),
                IsFile = true,
                IsImageFile = ImageUtil.IsImageFile(filePath),
                CreateDate = createDate,
                UpdateDate = updateDate,
                TakenDate = await Instance<IImageFileTakenDateCacher>.Value.Get(filePath).False(),
                SmallIcon = Instance<IFileIconCacher>.Value.GetSmallFileIcon(filePath),
                ExtraLargeIcon = Instance<IFileIconCacher>.Value.GetExtraLargeFileIcon(filePath),
                JumboIcon = new IconImage(Instance<IFileIconCacher>.Value.GetJumboFileIcon(filePath)),
            };

            if (!isGetThumbnail)
            {
                return info;
            }

            var thumbnailBuffer = await Instance<IThumbnailCacher>.Value.GetCache(filePath).False();
            if (!thumbnailBuffer.IsEmpry
                && thumbnailBuffer.ThumbnailBuffer != null)
            {
                info.ThumbnailImage = new OpenCVImage(
                    filePath,
                    ThumbnailUtil.ReadBufferToMat(thumbnailBuffer.ThumbnailBuffer));
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

        private async ValueTask<FileShallowInfoEntity> GetDirectoryInfo(
            string filePath, bool isGetThumbnail, DateTime addDate)
        {
            var (createDate, updateDate) = FileUtil.GetDirectoryInfo(filePath);

            var info = new FileShallowInfoEntity
            {
                RgistrationDate = addDate,
                FilePath = filePath,
                FileName = FileUtil.GetFileName(filePath),
                IsFile = false,
                IsImageFile = false,
                CreateDate = createDate,
                UpdateDate = updateDate,
                TakenDate = DateTimeExtensions.EMPTY,
                SmallIcon = Instance<IFileIconCacher>.Value.SmallDirectoryIcon,
                ExtraLargeIcon = Instance<IFileIconCacher>.Value.ExtraLargeDirectoryIcon,
                JumboIcon = new IconImage((Bitmap)Instance<IFileIconCacher>.Value.JumboDirectoryIcon),
            };

            if (!isGetThumbnail)
            {
                return info;
            }

            var thumbnailBuffer = await Instance<IThumbnailCacher>.Value.GetCache(filePath).False();
            if (!thumbnailBuffer.IsEmpry
                && thumbnailBuffer.ThumbnailBuffer != null)
            {
                info.ThumbnailImage = new OpenCVImage(
                    filePath,
                    ThumbnailUtil.ReadBufferToMat(thumbnailBuffer.ThumbnailBuffer));
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
            string filePath, DateTime addDate)
        {
            var (createDate, updateDate) = FileUtil.GetDirectoryInfo(filePath);

            return new FileShallowInfoEntity
            {
                RgistrationDate = addDate,
                FilePath = filePath,
                FileName = FileUtil.GetFileName(filePath),
                IsFile = false,
                IsImageFile = false,
                CreateDate = createDate,
                UpdateDate = updateDate,
                TakenDate = DateTimeExtensions.EMPTY,
                SmallIcon = Instance<IFileIconCacher>.Value.GetSmallDriveIcon(filePath),
                ExtraLargeIcon = Instance<IFileIconCacher>.Value.GetExtraLargeDriveIcon(filePath),
                JumboIcon = new IconImage((Bitmap)Instance<IFileIconCacher>.Value.GetJumboDriveIcon(filePath)),
            };
        }

        private FileShallowInfoEntity GetSystemRootInfo(
            string filePath, DateTime addDate)
        {
            return new FileShallowInfoEntity
            {
                RgistrationDate = addDate,
                FilePath = filePath,
                FileName = FileUtil.GetFileName(filePath),
                IsFile = false,
                IsImageFile = false,
                CreateDate = DateTimeExtensions.EMPTY,
                UpdateDate = DateTimeExtensions.EMPTY,
                TakenDate = DateTimeExtensions.EMPTY,
                SmallIcon = Instance<IFileIconCacher>.Value.SmallPCIcon,
                ExtraLargeIcon = Instance<IFileIconCacher>.Value.LargePCIcon,
                JumboIcon = new IconImage((Bitmap)Instance<IFileIconCacher>.Value.LargePCIcon),
            };
        }
    }
}
