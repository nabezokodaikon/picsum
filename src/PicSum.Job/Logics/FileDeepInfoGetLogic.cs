using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using PicSum.Job.Entities;
using SWF.Core.Base;
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
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class FileDeepInfoGetLogic
        : AbstractAsyncLogic
    {
        public FileDeepInfoGetLogic(IAsyncJob job)
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
                info.FileIcon = Instance<IFileIconCacher>.Value.LargePCIcon;
            }
            else
            {
                info.UpdateDate = FileUtil.GetUpdateDate(filePath);
                info.IsFile = FileUtil.IsFile(filePath);
                if (info.IsFile)
                {
                    info.IsImageFile = FileUtil.IsImageFile(filePath);
                    info.FileSize = FileUtil.GetFileSize(filePath);
                    info.FileIcon = Instance<IFileIconCacher>.Value.GetJumboFileIcon(filePath);
                }
                else
                {
                    info.IsImageFile = false;
                    info.FileSize = null;
                    if (FileUtil.IsDrive(filePath))
                    {
                        info.FileIcon = Instance<IFileIconCacher>.Value.GetJumboDriveIcon(filePath);
                    }
                    else if (FileUtil.IsDirectory(filePath))
                    {
                        info.FileIcon = Instance<IFileIconCacher>.Value.JumboDirectoryIcon;
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
                        this.CheckCancel();

                        using (var cvImage = this.ReadImageFile(filePath))
                        {
                            this.CheckCancel();

                            Bitmap? thumbnail = null;
                            try
                            {
                                thumbnail = ThumbnailUtil.CreateThumbnail(
                                    cvImage,
                                    Math.Max(thumbSize.Width, thumbSize.Height),
                                    ImageSizeMode.Original);

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
                                info.ImageSize = new(srcSize.Width, srcSize.Height);
                            }
                            catch (JobCancelException)
                            {
                                thumbnail?.Dispose();
                                throw;
                            }
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
                            this.CheckCancel();

                            using (var cvImage = this.ReadImageFile(firstImageFile))
                            {
                                this.CheckCancel();

                                Bitmap? thumbnail = null;
                                try
                                {
                                    thumbnail = ThumbnailUtil.CreateThumbnail(
                                        cvImage,
                                        Math.Max(cvImage.Width, cvImage.Height),
                                        ImageSizeMode.Original);

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
                                    info.ImageSize = new(srcSize.Width, srcSize.Height);
                                }
                                catch (JobCancelException)
                                {
                                    thumbnail?.Dispose();
                                    throw;
                                }
                            }
                        }
                    }
                }
            }

            this.CheckCancel();

            var sql = new FileInfoReadSql(info.FilePath);
            var dto = Instance<IFileInfoDB>.Value.ReadLine<FileInfoDto>(sql);
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
                return Instance<IImageFileSizeCacher>.Value.Get(filePath).Size;
            }
            catch (FileUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.Job.ID, ex));
                return ImageUtil.EMPTY_SIZE;
            }
            catch (ImageUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.Job.ID, ex));
                return ImageUtil.EMPTY_SIZE;
            }
        }

        internal CvImage ReadImageFile(string filePath)
        {
            try
            {
                return Instance<IImageFileCacher>.Value.GetCvImage(filePath);
            }
            catch (FileUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.Job.ID, ex));
                return CvImage.EMPTY;
            }
            catch (ImageUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.Job.ID, ex));
                return CvImage.EMPTY;
            }
        }
    }
}
