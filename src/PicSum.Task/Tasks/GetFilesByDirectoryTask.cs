using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Logics;
using PicSum.Task.Entities;
using PicSum.Task.Results;
using SWF.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// ファイルをフォルダで検索します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetFilesByDirectoryTask
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
            var getFilesLogic = new GetFilesAndSubFoldersLogic(this);
            try
            {
                fileList = getFilesLogic.Execute(param.Value);
            }
            catch (FileUtilException ex)
            {
                throw new TaskException(this.ID, ex);
            }

            var getInfoLogic = new GetFileShallowInfoLogic(this);
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

            var getDirectoryStateLogic = new GetDirectoryStateLogic(this);
            var directoryState = getDirectoryStateLogic.Execute(param.Value);

            result.FileInfoList = infoList;
            result.DirectoryState = directoryState;

            this.Callback(result);
        }
    }
}
