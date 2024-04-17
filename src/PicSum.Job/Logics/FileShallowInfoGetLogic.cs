using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Entities;
using SWF.Common;
using System;
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
                info.FilePath = filePath;
                info.FileName = FileUtil.GetFileName(filePath);
                info.IsFile = true;
                info.IsImageFile = FileUtil.IsImageFile(filePath);
                info.UpdateDate = FileUtil.GetUpdateDate(filePath);
                info.LargeIcon = FileIconCash.GetLargeFileIcon(info.FilePath);
                info.SmallIcon = FileIconCash.GetSmallFileIcon(info.FilePath);
            }
            else if (FileUtil.IsDirectory(filePath))
            {
                info.FilePath = filePath;
                info.FileName = FileUtil.GetFileName(filePath);
                info.IsFile = false;
                info.IsImageFile = false;
                info.UpdateDate = FileUtil.GetUpdateDate(filePath);
                info.LargeIcon = FileIconCash.LargeDirectoryIcon;
                info.SmallIcon = FileIconCash.SmallDirectoryIcon;
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
