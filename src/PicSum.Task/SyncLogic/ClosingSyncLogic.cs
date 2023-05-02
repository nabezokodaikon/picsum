﻿using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.SyncTask;
using PicSum.Data.DatabaseAccessor.Connection;

namespace PicSum.Task.SyncLogic
{
    /// <summary>
    /// 終了同期ロジック
    /// </summary>
    internal sealed class ClosingSyncLogic
        : AbstractSyncLogic
    {
        public void Execute()
        {
            DatabaseManager<FileInfoConnection>.Close();
            DatabaseManager<ThumbnailConnection>.Close();
        }
    }
}
