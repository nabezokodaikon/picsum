using System;
using System.Collections.Generic;
using System.IO;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// ファイルをフォルダで検索します。
    /// </summary>
    public class SearchFileByFolderAsyncFacade
        : TwoWayFacadeBase<SingleValueEntity<string>, SearchFolderResultEntity>
    {
        public override void Execute(SingleValueEntity<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            SearchFolderResultEntity result = new SearchFolderResultEntity();
            result.FolderPath = param.Value;

            IList<string> fileList = null;
            if (string.IsNullOrEmpty(param.Value))
            {
                GetDrivesAsyncLogic getFilesLogic = new GetDrivesAsyncLogic(this);
                fileList = getFilesLogic.Execute();
            }
            else
            {
                GetFilesAndSubFoldersAsyncLogic getFilesLogic = new GetFilesAndSubFoldersAsyncLogic(this);
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

            GetFolderStateAsyncLogic getFolderStateLogic = new GetFolderStateAsyncLogic(this);
            FolderStateEntity folderState = getFolderStateLogic.Execute(param.Value);

            result.FileInfoList = infoList;
            result.FolderState = folderState;

            OnCallback(result);
        }
    }
}
