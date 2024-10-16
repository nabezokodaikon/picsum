using PicSum.Job.Entities;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイルの浅い情報を取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class FileShallowInfoGetLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public FileShallowInfoEntity Execute(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var info = new FileShallowInfoEntity
            {
                RgistrationDate = null
            };

            if (FileUtil.IsSystemRoot(filePath))
            {
                info.FilePath = filePath;
                info.FileName = FileUtil.GetFileName(filePath);
                info.IsFile = false;
                info.IsImageFile = false;
                info.UpdateDate = null;
                info.LargeIcon = FileIconCash.LargeMyComputerIcon;
                info.SmallIcon = FileIconCash.SmallMyComputerIcon;
            }
            else if (FileUtil.IsDrive(filePath))
            {
                info.FilePath = filePath;
                info.FileName = FileUtil.GetFileName(filePath);
                info.IsFile = false;
                info.IsImageFile = false;
                info.UpdateDate = FileUtil.GetUpdateDate(filePath);
                info.LargeIcon = FileIconCash.GetLargeDriveIcon(info.FilePath);
                info.SmallIcon = FileIconCash.GetSmallDriveIcon(info.FilePath);
            }
            else if (FileUtil.IsFile(filePath))
            {
                var thumbnailGetLogic = new ThumbnailGetLogic(job);
                var thumbnailBuffer = thumbnailGetLogic.GetOnlyCache(filePath, 248, 248);
                var thumbnailImage = thumbnailBuffer switch
                {
                    var t when t != ThumbnailBufferEntity.EMPTY && t.ThumbnailBuffer != null =>
                        ImageUtil.ToImage(t.ThumbnailBuffer),
                    _ => null,
                };

                info.FilePath = filePath;
                info.FileName = FileUtil.GetFileName(filePath);
                info.IsFile = true;
                info.IsImageFile = FileUtil.IsImageFile(filePath);
                info.UpdateDate = FileUtil.GetUpdateDate(filePath);
                info.LargeIcon = FileIconCash.GetLargeFileIcon(info.FilePath);
                info.SmallIcon = FileIconCash.GetSmallFileIcon(info.FilePath);
                info.ThumbnailImage = thumbnailImage;
            }
            else if (FileUtil.IsDirectory(filePath))
            {
                var thumbnailGetLogic = new ThumbnailGetLogic(job);
                var thumbnailBuffer = thumbnailGetLogic.GetOnlyCache(filePath, 248, 248);
                var thumbnailImage = thumbnailBuffer switch
                {
                    var t when t != ThumbnailBufferEntity.EMPTY && t.ThumbnailBuffer != null =>
                        ImageUtil.ToImage(t.ThumbnailBuffer),
                    _ => null,
                };

                info.FilePath = filePath;
                info.FileName = FileUtil.GetFileName(filePath);
                info.IsFile = false;
                info.IsImageFile = false;
                info.UpdateDate = FileUtil.GetUpdateDate(filePath);
                info.LargeIcon = FileIconCash.LargeDirectoryIcon;
                info.SmallIcon = FileIconCash.SmallDirectoryIcon;
                info.ThumbnailImage = thumbnailImage;
            }
            else
            {
                throw new ArgumentException($"不正なファイルパスです。'{filePath}'", nameof(filePath));
            }

            return info;
        }

        public FileShallowInfoEntity Execute(string filePath, DateTime registrationDate)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var info = this.Execute(filePath);
            info.RgistrationDate = registrationDate;

            return info;
        }
    }
}
