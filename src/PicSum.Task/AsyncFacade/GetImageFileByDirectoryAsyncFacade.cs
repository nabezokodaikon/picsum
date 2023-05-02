using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Paramter;
using PicSum.Task.Result;
using SWF.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// フォルダ内の画像ファイルを検索します。
    /// </summary>
    public sealed class GetImageFileByDirectoryAsyncFacade
        : TwoWayFacadeBase<GetImageFileByDirectoryParameter, GetImageFileByDirectoryResult>
    {
        public override void Execute(GetImageFileByDirectoryParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            if (string.IsNullOrEmpty(param.FilePath))
            {
                throw new ArgumentException("空文字は無効です。", nameof(param));
            }

            var result = new GetImageFileByDirectoryResult();
            if (FileUtil.IsDirectory(param.FilePath))
            {
                result.DirectoryPath = param.FilePath;
            }
            else if (FileUtil.IsFile(param.FilePath))
            {
                result.DirectoryPath = FileUtil.GetParentDirectoryPath(param.FilePath);
            }
            else
            {
                throw new ArgumentException("ファイルまたはフォルダのパスではありません。", nameof(param));
            }

            IList<string> filePathList = null;
            try
            {
                var getFilesLogic = new GetFilesAndSubDirectorysAsyncLogic(this);
                filePathList = getFilesLogic.Execute(result.DirectoryPath);
                result.DirectoryNotFoundException = null;
            }
            catch (DirectoryNotFoundException ex)
            {
                result.DirectoryNotFoundException = ex;
                this.OnCallback(result);
                return;
            }

            result.FilePathList = new List<string>();
            foreach (var filePath in filePathList)
            {
                if (FileUtil.IsImageFile(filePath))
                {
                    result.FilePathList.Add(filePath);
                }
            }

            if (result.FilePathList.Contains(param.FilePath))
            {
                result.SelectedFilePath = param.FilePath;
            }
            else
            {
                result.SelectedFilePath = string.Empty;
            }

            this.OnCallback(result);
        }
    }
}
