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
    /// フォルダ状態テーブルに登録します。
    /// </summary>
    internal class AddFolderStateAsyncLogic : AsyncLogicBase
    {
        public AddFolderStateAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

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

            CreationFolderStateSql sql = null;
            if (string.IsNullOrEmpty(folderState.SelectedFilePath))
            {
                sql = new CreationFolderStateSql(folderState.FolderPath, (int)folderState.SortTypeID, folderState.IsAscending);
            }
            else
            {
                sql = new CreationFolderStateSql(folderState.FolderPath, (int)folderState.SortTypeID, folderState.IsAscending, folderState.SelectedFilePath);
            }

            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
