using System;
using System.Collections.Generic;
using System.IO;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using PicSum.Task.Result;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// ファイルをフォルダで検索します。
    /// </summary>
    public class GetFileByDirectoryAsyncFacade
        : TwoWayFacadeBase<SingleValueEntity<string>, GetDirectoryResult>
    {
        public override void Execute(SingleValueEntity<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            GetDirectoryResult result = new GetDirectoryResult();
            result.DirectoryPath = param.Value;

            IList<string> fileList = null;
            if (string.IsNullOrEmpty(param.Value))
            {
                GetDrivesAsyncLogic getFilesLogic = new GetDrivesAsyncLogic(this);
                fileList = getFilesLogic.Execute();
            }
            else
            {
                GetFilesAndSubDirectorysAsyncLogic getFilesLogic = new GetFilesAndSubDirectorysAsyncLogic(this);
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
            }

            GetFileShallowInfoAsyncLogic getInfoLogic = new GetFileShallowInfoAsyncLogic(this);
            ListEntity<FileShallowInfoEntity> infoList = new ListEntity<FileShallowInfoEntity>();
            foreach (string file in fileList)
            {
                CheckCancel();

                FileShallowInfoEntity info = getInfoLogic.Execute(file);
                if (info != null)
                {
                    infoList.Add(info);
                }
            }

            GetDirectoryStateAsyncLogic getDirectoryStateLogic = new GetDirectoryStateAsyncLogic(this);
            DirectoryStateEntity directoryState = getDirectoryStateLogic.Execute(param.Value);

            result.FileInfoList = infoList;
            result.DirectoryState = directoryState;

            OnCallback(result);
        }
    }
}
