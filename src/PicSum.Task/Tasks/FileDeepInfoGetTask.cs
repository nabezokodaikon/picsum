using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Logics;
using PicSum.Task.Paramters;
using PicSum.Task.Results;
using SWF.Common;
using System;
using System.Runtime.Versioning;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// ファイルの深い情報取得非同期タスク
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class FileDeepInfoGetTask
        : AbstractTwoWayTask<FileDeepInfoGetParameter, FileDeepInfoGetResult>
    {
        protected override void Execute(FileDeepInfoGetParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            var result = new FileDeepInfoGetResult
            {
                FilePathList = param.FilePathList
            };

            if (param.FilePathList.Count == 1)
            {
                try
                {
                    var getInfoLogic = new FileDeepInfoGetLogic(this);
                    var filePath = param.FilePathList[0];
                    result.FileInfo = getInfoLogic.Execute(filePath, param.ThumbnailSize);
                }
                catch (FileUtilException ex)
                {
                    throw new TaskException(this.ID, ex);
                }
                catch (ImageUtilException ex)
                {
                    throw new TaskException(this.ID, ex);
                }
            }

            if (param.FilePathList.Count <= 997)
            {
                var logic = new FilesTagInfoGetLogic(this);
                result.TagInfoList = logic.Execute(result.FilePathList);
            }
            else
            {
                result.TagInfoList = [];
            }

            this.CheckCancel();

            this.Callback(result);
        }
    }
}
