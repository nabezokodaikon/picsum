using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using PicSum.Task.Result;
using System;
using System.Collections.Generic;
using System.IO;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// ファイルをフォルダで検索します。
    /// </summary>
    public sealed class GetFilesByDirectoryAsyncFacade
        : TwoWayFacadeBase<SingleValueEntity<string>, GetDirectoryResult>
    {
        public override void Execute(SingleValueEntity<string> param)
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
                result.DirectoryNotFoundException = null;
            }
            catch (DirectoryNotFoundException ex)
            {
                result.DirectoryNotFoundException = ex;
                OnCallback(result);
                return;
            }

            var getInfoLogic = new GetFileShallowInfoAsyncLogic(this);
            var infoList = new ListEntity<FileShallowInfoEntity>();
            foreach (var file in fileList)
            {
                this.CheckCancel();

                var info = getInfoLogic.Execute(file);
                if (info != null)
                {
                    infoList.Add(info);
                }
            }

            var getDirectoryStateLogic = new GetDirectoryStateAsyncLogic(this);
            var directoryState = getDirectoryStateLogic.Execute(param.Value);

            result.FileInfoList = infoList;
            result.DirectoryState = directoryState;

            this.OnCallback(result);
        }
    }
}
