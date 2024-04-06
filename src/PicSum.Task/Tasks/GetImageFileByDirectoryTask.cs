using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Logics;
using PicSum.Task.Paramters;
using PicSum.Task.Results;
using SWF.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// フォルダ内の画像ファイルを検索します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetImageFileByDirectoryTask
        : AbstractAsyncTask<GetImageFileByDirectoryParameter, GetImageFileByDirectoryResult>
    {
        protected override void Execute(GetImageFileByDirectoryParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var result = new GetImageFileByDirectoryResult();

            try
            {
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

                var getFilesLogic = new GetFilesAndSubFoldersLogic(this);
                var filePathList = getFilesLogic.Execute(result.DirectoryPath);

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
            }
            catch (FileUtilException ex)
            {
                throw new TaskException(this.ID, ex);
            }

            this.Callback(result);
        }
    }
}
