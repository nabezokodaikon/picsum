using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Entities;
using SWF.Common;
using System;
using System.Runtime.Versioning;

namespace PicSum.Task.Logics
{
    /// <summary>
    /// ファイルの浅い情報を取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class GetFileShallowInfoLogic
        : AbstractAsyncLogic
    {
        public GetFileShallowInfoLogic(IAsyncTask task)
            : base(task)
        {

        }

        public FileShallowInfoEntity Execute(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var info = new FileShallowInfoEntity();
            info.RgistrationDate = null;

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
                string ex = FileUtil.GetExtension(filePath);
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
                return null;
            }

            return info;
        }

        public FileShallowInfoEntity Execute(string filePath, DateTime registrationDate)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var info = this.Execute(filePath);
            info.RgistrationDate = registrationDate;

            return info;
        }
    }
}