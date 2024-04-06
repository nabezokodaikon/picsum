using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using PicSum.Task.Result;
using SWF.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace PicSum.Task.AsyncTask
{
    /// <summary>
    /// ファイルをフォルダで検索します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetFilesByDirectoryAsyncTask
        : AbstractAsyncTask<ValueParameter<string>, GetDirectoryResult>
    {
        protected override void Execute(ValueParameter<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var result = new GetDirectoryResult();
            result.DirectoryPath = param.Value;

            IList<string> fileList;
            var getFilesLogic = new GetFilesAndSubDirectorysAsyncLogic(this);
            try
            {
                fileList = getFilesLogic.Execute(param.Value);
            }
            catch (FileUtilException ex)
            {
                throw new TaskException(this.ID, ex);
            }

            var getInfoLogic = new GetFileShallowInfoAsyncLogic(this);
            var infoList = new ListEntity<FileShallowInfoEntity>();
            foreach (var file in fileList)
            {
                this.CheckCancel();

                try
                {
                    var info = getInfoLogic.Execute(file);
                    if (info != null)
                    {
                        infoList.Add(info);
                    }
                }
                catch (FileUtilException)
                {
                    continue;
                }
            }

            var getDirectoryStateLogic = new GetDirectoryStateAsyncLogic(this);
            var directoryState = getDirectoryStateLogic.Execute(param.Value);

            result.FileInfoList = infoList;
            result.DirectoryState = directoryState;

            this.Callback(result);
        }
    }
}
