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
    /// ファイルの深い情報取得ロジック
    /// </summary>

    internal sealed class FileDeepInfoGetLogic
        : AbstractLogic
    {
        public FileDeepInfoGetLogic(IJob job)
            : base(job)
        {

        }

        public async ValueTask<FileDeepInfoEntity> Get(
            string filePath, Size thumbSize, bool isReadThumbnail)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (!FileUtil.CanAccess(filePath))
            {
                return FileDeepInfoEntity.ERROR;
            }

            if (FileUtil.IsExistsFile(filePath))
            {
                return await this.GetFileInfo(
                    filePath, thumbSize, isReadThumbnail).False();
            }
            else if (FileUtil.IsSystemRoot(filePath))
            {
                return await this.GetSystemRootInfo().False();
            }
            else if (FileUtil.IsExistsDrive(filePath))
            {
                return await this.GetDriveInfo(filePath).False();
            }
            else if (FileUtil.IsExistsDirectory(filePath))
            {
                return await this.GetDirectoryInfo(
                    filePath, thumbSize, isReadThumbnail).False();
            }

            return FileDeepInfoEntity.ERROR;
        }

        private async ValueTask<FileDeepInfoEntity> GetSystemRootInfo()
        {
            return new FileDeepInfoEntity
            {
                FilePath = FileUtil.ROOT_DIRECTORY_PATH,
                FileName = FileUtil.ROOT_DIRECTORY_NAME,
                FileType = FileUtil.ROOT_DIRECTORY_TYPE_NAME,
                CreateDate = DateTimeExtensions.EMPTY,
                UpdateDate = DateTimeExtensions.EMPTY,
                TakenDate = DateTimeExtensions.EMPTY,
                IsFile = false,
                IsImageFile = false,
                FileSize = 0,
                FilesAndDirectoriesCount = await Instance<IFilesAndDirectoriesCountCacher>.Value.GetOrCreate(FileUtil.ROOT_DIRECTORY_PATH).False(),
                FileIcon = Instance<IFileIconCacher>.Value.LargePCIcon,
                ImageSize = ImageUtil.EMPTY_SIZE,
            };
        }

        private async ValueTask<FileDeepInfoEntity> GetDriveInfo(string filePath)
        {
            var (createDate, updateDate) = FileUtil.GetDirectoryInfo(filePath);

            return new FileDeepInfoEntity
            {
                FilePath = filePath,
                FileName = FileUtil.GetFileName(filePath),
                FileType = FileUtil.GetTypeName(filePath),
                CreateDate = createDate,
                UpdateDate = updateDate,
                TakenDate = DateTimeExtensions.EMPTY,
                IsFile = false,
                IsImageFile = false,
                FileSize = 0,
                FilesAndDirectoriesCount = await Instance<IFilesAndDirectoriesCountCacher>.Value.GetOrCreate(filePath).False(),
                FileIcon = Instance<IFileIconCacher>.Value.GetJumboDriveIcon(filePath),
                ImageSize = ImageUtil.EMPTY_SIZE,
            };
        }

        private async ValueTask<FileDeepInfoEntity> GetDirectoryInfo(
            string filePath, Size thumbSize, bool isReadThumbnail)
        {
            var (createDate, updateDate) = FileUtil.GetDirectoryInfo(filePath);

            var info = new FileDeepInfoEntity
            {
                FilePath = filePath,
                FileName = FileUtil.GetFileName(filePath),
                FileType = FileUtil.GetTypeName(filePath),
                CreateDate = createDate,
                UpdateDate = updateDate,
                TakenDate = DateTimeExtensions.EMPTY,
                IsFile = false,
                IsImageFile = false,
                FileSize = 0,
                FilesAndDirectoriesCount = await Instance<IFilesAndDirectoriesCountCacher>.Value.GetOrCreate(filePath).False(),
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

            var cache = await Instance<IImageFileSizeCacher>.Value.GetOrCreate(firstImageFile).False();
            info.ImageSize = new(cache.Size.Width, cache.Size.Height);

            this.ThrowIfJobCancellationRequested();

            var thumbnail = await this.ReadImageFile(firstImageFile, AppConstants.DEFAULT_ZOOM_VALUE).False();

            this.ThrowIfJobCancellationRequested();

            info.Thumbnail = new()
            {
                FilePath = info.FilePath,
                FileUpdateDate = info.UpdateDate,
                ThumbnailImage = thumbnail,
                ThumbnailWidth = thumbSize.Width,
                ThumbnailHeight = thumbSize.Height,
                SourceWidth = cache.Size.Width,
                SourceHeight = cache.Size.Height,
            };

            return info;
        }

        private async ValueTask<FileDeepInfoEntity> GetFileInfo(
            string filePath, Size thumbSize, bool isReadThumbnail)
        {
            var (createDate, updateDate, fileSize) = FileUtil.GetFileInfo(filePath);

            var info = new FileDeepInfoEntity
            {
                FilePath = filePath,
                FileName = FileUtil.GetFileName(filePath),
                FileType = FileUtil.GetTypeName(filePath),
                CreateDate = createDate,
                UpdateDate = updateDate,
                TakenDate = await this.GetTakenDate(filePath).False(),
                IsFile = true,
                FileSize = fileSize,
                FilesAndDirectoriesCount = FilesAndDirectoriesCountEntity.EMPTY,
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

            info.TakenDate = await this.GetOrCreateTaken(filePath).False();
            this.ThrowIfJobCancellationRequested();

            var cache = await Instance<IImageFileSizeCacher>.Value.GetOrCreate(filePath).False();
            info.ImageSize = new(cache.Size.Width, cache.Size.Height);

            this.ThrowIfJobCancellationRequested();

            var thumbnail = await this.ReadImageFile(filePath, AppConstants.DEFAULT_ZOOM_VALUE).False();
            info.IsImageFile = true;

            this.ThrowIfJobCancellationRequested();

            info.Thumbnail = new()
            {
                FilePath = info.FilePath,
                FileUpdateDate = info.UpdateDate,
                ThumbnailImage = thumbnail,
                ThumbnailWidth = thumbSize.Width,
                ThumbnailHeight = thumbSize.Height,
                SourceWidth = cache.Size.Width,
                SourceHeight = cache.Size.Height
            };

            return info;
        }

        private async ValueTask<OpenCVImage> ReadImageFile(string filePath, float zoomValue)
        {
            try
            {
                using (Measuring.Time(false, "FileDeepInfoGetLogic.ReadImageFile Get Cache"))
                {
                    var image = await Instance<IImageFileCacher>.Value.GetCache(filePath, zoomValue).False();
                    if (!image.IsEmpry)
                    {
                        return image;
                    }
                }

                using (Measuring.Time(false, "ImageFileReadLogic.ReadImageFile Read File"))
                {
                    using (var bmp = await ImageUtil.ReadImageFile(filePath).False())
                    {
                        return new OpenCVImage(
                            filePath, OpenCVUtil.ToMat(bmp), zoomValue);
                    }
                }
            }
            catch (Exception ex) when (
                ex is FileUtilException ||
                ex is ImageUtilException)
            {
                this.WriteErrorLog(ex);
                return OpenCVImage.EMPTY;
            }
        }

        private async ValueTask<DateTime> GetOrCreateTaken(string filePath)
        {
            try
            {
                return await Instance<IImageFileTakenDateCacher>.Value.GetOrCreate(filePath).False();
            }
            catch (ImageUtilException ex)
            {
                this.WriteErrorLog(ex);
                return DateTimeExtensions.EMPTY;
            }
        }

        private async ValueTask<DateTime> GetTakenDate(string filePath)
        {
            try
            {
                return await Instance<IImageFileTakenDateCacher>.Value.Get(filePath).False();
            }
            catch (ImageUtilException ex)
            {
                this.WriteErrorLog(ex);
                return DateTimeExtensions.EMPTY;
            }
        }
    }
}
