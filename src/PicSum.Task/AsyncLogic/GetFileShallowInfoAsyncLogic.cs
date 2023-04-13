using System;
using System.Collections.Generic;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.Entity;
using SWF.Common;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// ファイルの浅い情報を取得します。
    /// </summary>
    internal class GetFileShallowInfoAsyncLogic : AsyncLogicBase
    {
        public GetFileShallowInfoAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public FileShallowInfoEntity Execute(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            FileShallowInfoEntity info = new FileShallowInfoEntity();
            info.FilePath = filePath;
            info.FileName = FileUtil.GetFileName(filePath);

            if (string.IsNullOrEmpty(filePath))
            {
                info.IsFile = false;
                info.IsImageFile = false;
                info.UpdateDate = null;
                info.CreateDate = null;
                info.LargeIcon = FileIconCash.LargeMyComputerIcon;
                info.SmallIcon = FileIconCash.SmallMyComputerIcon;
            }
            else if (FileUtil.IsDrive(filePath))
            {
                info.IsFile = false;
                info.IsImageFile = false;
                info.UpdateDate = FileUtil.GetUpdateDate(filePath);
                info.CreateDate = FileUtil.GetCreateDate(filePath);
                info.LargeIcon = FileIconCash.GetLargeDriveIcon(info.FilePath);
                info.SmallIcon = FileIconCash.GetSmallDriveIcon(info.FilePath);
            }
            else if (FileUtil.IsFile(filePath))
            {
                info.IsFile = true;
                string ex = FileUtil.GetExtension(filePath);
                info.IsImageFile = ImageUtil.ImageFileExtensionList.Contains(ex);
                info.UpdateDate = FileUtil.GetUpdateDate(filePath);
                info.CreateDate = FileUtil.GetCreateDate(filePath);
                info.LargeIcon = FileIconCash.GetLargeFileIcon(info.FilePath);
                info.SmallIcon = FileIconCash.GetSmallFileIcon(info.FilePath);
            }
            else if (FileUtil.IsDirectory(filePath))
            {
                info.IsFile = false;
                info.IsImageFile = false;
                info.UpdateDate = FileUtil.GetUpdateDate(filePath);
                info.CreateDate = FileUtil.GetCreateDate(filePath);
                info.LargeIcon = FileIconCash.LargeDirectoryIcon;
                info.SmallIcon = FileIconCash.SmallDirectoryIcon;
            }
            else
            {
                return null;
            }

            return info;
        }
    }
}
