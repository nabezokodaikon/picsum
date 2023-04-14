﻿using System;
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
    internal class UpdateDirectoryStateAsyncLogic : AsyncLogicBase
    {
        public UpdateDirectoryStateAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

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

            if (directoryState.SortTypeID == SortTypeID.Default)
            {
                throw new ArgumentException("ソートIDがデフォルトです。", "directoryState");
            }

            UpdateDirectoryStateSql sql = null;
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