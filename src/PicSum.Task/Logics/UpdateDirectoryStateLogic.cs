using PicSum.Core.Base.Conf;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using PicSum.Task.Parameters;
using System;

namespace PicSum.Task.Logics
{
    /// <summary>
    /// フォルダ状態テーブルを更新します。
    /// </summary>
    internal sealed class UpdateDirectoryStateLogic
        : AbstractAsyncLogic
    {
        public UpdateDirectoryStateLogic(IAsyncTask task)
            : base(task)
        {

        }

        public bool Execute(DirectoryStateParameter directoryState)
        {
            if (directoryState == null)
            {
                throw new ArgumentNullException(nameof(directoryState));
            }

            if (directoryState.DirectoryPath == null)
            {
                throw new ArgumentException("フォルダパスがNULLです。", nameof(directoryState));
            }

            if (directoryState.SelectedFilePath == null)
            {
                throw new ArgumentException("選択ファイルパスがNULLです。", nameof(directoryState));
            }

            if (directoryState.SortTypeID == SortTypeID.Default)
            {
                throw new ArgumentException("ソートIDがデフォルトです。", nameof(directoryState));
            }

            UpdateDirectoryStateSql sql;
            if (string.IsNullOrEmpty(directoryState.SelectedFilePath))
            {
                sql = new UpdateDirectoryStateSql(directoryState.DirectoryPath, (int)directoryState.SortTypeID, directoryState.IsAscending);
            }
            else
            {
                sql = new UpdateDirectoryStateSql(directoryState.DirectoryPath, (int)directoryState.SortTypeID, directoryState.IsAscending, directoryState.SelectedFilePath);
            }

            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}