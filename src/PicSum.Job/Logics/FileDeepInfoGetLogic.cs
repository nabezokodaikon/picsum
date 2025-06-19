using PicSum.Job.Common;
using PicSum.Job.Entities;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;
using System.Drawing;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイルの深い情報取得ロジック
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class FileDeepInfoGetLogic
        : AbstractAsyncLogic
    {
        public FileDeepInfoGetLogic(IAsyncJob job)
            : base(job)
        {

        }

        public FileDeepInfoEntity Get(
            string filePath, Size thumbSize, bool isReadThumbnail)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (!FileUtil.CanAccess(filePath))
            {
                return FileDeepInfoEntity.ERROR;
            }

            if (FileUtil.IsExistsFile(filePath))
            {
                return this.GetFileInfo(
                    filePath, thumbSize, isReadThumbnail);
            }
            else if (FileUtil.IsExistsDirectory(filePath))
            {
                return this.GetDirectoryInfo(
                    filePath, thumbSize, isReadThumbnail);
            }
            else if (FileUtil.IsExistsDrive(filePath))
            {
                return this.GetDriveInfo(filePath);
            }
            else if (FileUtil.IsSystemRoot(filePath))
            {
                return this.GetSystemRootInfo();
            }

            return FileDeepInfoEntity.ERROR;
        }

        private FileDeepInfoEntity GetSystemRootInfo()
        {
            return new FileDeepInfoEntity
            {
                FilePath = FileUtil.ROOT_DIRECTORY_PATH,
                FileName = FileUtil.ROOT_DIRECTORY_NAME,
                FileType = FileUtil.ROOT_DIRECTORY_TYPE_NAME,
                UpdateDate = FileUtil.ROOT_DIRECTORY_DATETIME,
                IsFile = false,
                IsImageFile = false,
                FileSize = 0,
                FileIcon = Instance<IFileIconCacher>.Value.LargePCIcon,
                ImageSize = ImageUtil.EMPTY_SIZE,
            };
        }

        private FileDeepInfoEntity GetDriveInfo(string filePath)
        {
            return new FileDeepInfoEntity
            {
                FilePath = filePath,
                FileName = FileUtil.GetFileName(filePath),
                FileType = FileUtil.GetTypeName(filePath),
                UpdateDate = FileUtil.GetUpdateDate(filePath),
                IsFile = false,
                IsImageFile = false,
                FileSize = 0,
                FileIcon = Instance<IFileIconCacher>.Value.GetJumboDriveIcon(filePath),
                ImageSize = ImageUtil.EMPTY_SIZE,
            };
        }

        private FileDeepInfoEntity GetDirectoryInfo(
            string filePath, Size thumbSize, bool isReadThumbnail)
        {
            var info = new FileDeepInfoEntity
            {
                FilePath = filePath,
                FileName = FileUtil.GetFileName(filePath),
                FileType = FileUtil.GetTypeName(filePath),
                UpdateDate = FileUtil.GetUpdateDate(filePath),
                IsFile = false,
                IsImageFile = false,
                FileSize = 0,
                FileIcon = Instance<IFileIconCacher>.Value.JumboDirectoryIcon,
            };

            if (!isReadThumbnail)
            {
                return info;
            }

            var firstImageFile = ImageUtil.GetFirstImageFilePath(filePath);
            if (string.IsNullOrEmpty(firstImageFile))
            {
                return info;
            }

            var srcSize = Instance<IImageFileSizeCacher>.Value.GetOrCreate(firstImageFile).Size;
            info.ImageSize = new(srcSize.Width, srcSize.Height);

            this.CheckCancel();

            var thumbnail = this.GetThumbnail(firstImageFile, thumbSize);

            this.CheckCancel();

            info.Thumbnail = new()
            {
                FilePath = info.FilePath,
                FileUpdatedate = info.UpdateDate,
                ThumbnailImage = thumbnail,
                ThumbnailWidth = thumbSize.Width,
                ThumbnailHeight = thumbSize.Height,
                SourceWidth = srcSize.Width,
                SourceHeight = srcSize.Height,
            };

            return info;
        }

        private FileDeepInfoEntity GetFileInfo(
            string filePath, Size thumbSize, bool isReadThumbnail)
        {
            var info = new FileDeepInfoEntity
            {
                FilePath = filePath,
                FileName = FileUtil.GetFileName(filePath),
                FileType = FileUtil.GetTypeName(filePath),
                UpdateDate = FileUtil.GetUpdateDate(filePath),
                IsFile = true,
                FileSize = FileUtil.GetFileSize(filePath),
                FileIcon = Instance<IFileIconCacher>.Value.GetJumboFileIcon(filePath),
            };

            var isImageFile = ImageUtil.IsImageFile(filePath);
            if (!isImageFile)
            {
                info.IsImageFile = false;
                return info;
            }

            if (!isReadThumbnail)
            {
                info.IsImageFile = true;
                return info;
            }

            var srcSize = Instance<IImageFileSizeCacher>.Value.GetOrCreate(filePath).Size;
            info.ImageSize = new(srcSize.Width, srcSize.Height);

            this.CheckCancel();

            var thumbnail = this.GetThumbnail(filePath, thumbSize);
            info.IsImageFile = true;

            this.CheckCancel();

            info.Thumbnail = new()
            {
                FilePath = info.FilePath,
                FileUpdatedate = info.UpdateDate,
                ThumbnailImage = thumbnail,
                ThumbnailWidth = thumbSize.Width,
                ThumbnailHeight = thumbSize.Height,
                SourceWidth = srcSize.Width,
                SourceHeight = srcSize.Height
            };

            return info;
        }

        private CvImage GetThumbnail(string filePath, Size thumbSize)
        {
            var cache = Instance<IThumbnailCacher>.Value.GetCache(filePath);
            if (cache != ThumbnailCacheEntity.EMPTY
                && cache.ThumbnailBuffer != null
                && cache.ThumbnailWidth >= thumbSize.Width
                && cache.ThumbnailHeight >= thumbSize.Height)
            {
                return new CvImage(
                    filePath,
                    ThumbnailUtil.ToImage(cache.ThumbnailBuffer));
            }
            else
            {
                return Instance<IImageFileCacher>.Value.GetImage(
                    filePath,
                    AppConstants.DEFAULT_ZOOM_VALUE);
            }
        }
    }
}
