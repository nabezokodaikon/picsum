using System;
using PicSum.Core.Base.Conf;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// フォルダ状態テーブルを更新します。
    /// </summary>
    internal class UpdateFolderStateAsyncLogic : AsyncLogicBase
    {
        public UpdateFolderStateAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public bool Execute(FolderStateEntity folderState)
        {
            if (folderState == null)
            {
                throw new ArgumentNullException("folderState");
            }

            if (folderState.FolderPath == null)
            {
                throw new ArgumentException("フォルダパスがNULLです。", "folderState");
            }

            if (folderState.SelectedFilePath == null)
            {
                throw new ArgumentException("選択ファイルパスがNULLです。", "folderState");
            }

            if (folderState.SortTypeID == SortTypeID.Default)
            {
                throw new ArgumentException("ソートIDがデフォルトです。", "folderState");
            }

            UpdateFolderStateSql sql = null;
            if (string.IsNullOrEmpty(folderState.SelectedFilePath))
            {
                sql = new UpdateFolderStateSql(folderState.FolderPath, (int)folderState.SortTypeID, folderState.IsAscending);
            }
            else
            {
                sql = new UpdateFolderStateSql(folderState.FolderPath, (int)folderState.SortTypeID, folderState.IsAscending, folderState.SelectedFilePath);
            }

            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
