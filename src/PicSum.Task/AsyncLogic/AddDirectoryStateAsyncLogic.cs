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
    internal class AddDirectoryStateAsyncLogic : AbstractAsyncLogic
    {
        public AddDirectoryStateAsyncLogic(AbstractAsyncFacade facade) : base(facade) { }

        public bool Execute(DirectoryStateEntity directoryState)
        {
            if (directoryState == null)
            {
                throw new ArgumentNullException("directoryState");
            }

            if (directoryState.DirectoryPath == null)
            {
                throw new ArgumentException("フォルダパスがNULLです。", "directoryState");
            }

            if (directoryState.SelectedFilePath == null)
            {
                throw new ArgumentException("選択ファイルパスがNULLです。", "directoryState");
            }

            CreationDirectoryStateSql sql = null;
            if (string.IsNullOrEmpty(directoryState.SelectedFilePath))
            {
                sql = new CreationDirectoryStateSql(directoryState.DirectoryPath, (int)directoryState.SortTypeID, directoryState.IsAscending);
            }
            else
            {
                sql = new CreationDirectoryStateSql(directoryState.DirectoryPath, (int)directoryState.SortTypeID, directoryState.IsAscending, directoryState.SelectedFilePath);
            }

            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
