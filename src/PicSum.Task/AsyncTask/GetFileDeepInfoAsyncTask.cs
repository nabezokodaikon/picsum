using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using PicSum.Task.Result;
using SWF.Common;
using System;
using System.IO;
using System.Runtime.Versioning;

namespace PicSum.Task.AsyncTask
{
    /// <summary>
    /// ファイルの深い情報取得非同期タスク
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetFileDeepInfoAsyncTask
        : TwoWayTaskBase<GetFileDeepInfoParameter, GetFileDeepInfoResult>
    {
        public override void Execute(GetFileDeepInfoParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var result = new GetFileDeepInfoResult();

            result.FilePathList = param.FilePathList;

            if (param.FilePathList.Count == 1)
            {
                try
                {
                    var getInfoLogic = new GetFileDeepInfoAsyncLogic(this);
                    var filePath = param.FilePathList[0];
                    result.FileInfo = getInfoLogic.Execute(filePath, param.ThumbnailSize);
                }
                catch (FileUtilException)
                {
                    return;
                }
                catch (ImageUtilException)
                {
                    return;
                }
            }

            if (param.FilePathList.Count <= 997)
            {
                var logic = new GetFileTagInfoAsyncLogic(this);
                result.TagInfoList = logic.Execute(result.FilePathList);
            }
            else
            {
                result.TagInfoList = new ListEntity<FileTagInfoEntity>();
            }

            this.CheckCancel();

            this.OnCallback(result);
        }
    }
}
