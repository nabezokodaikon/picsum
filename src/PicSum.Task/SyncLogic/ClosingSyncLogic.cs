using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.SyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;

namespace PicSum.Task.SyncLogic
{
    /// <summary>
    /// 終了同期ロジック
    /// </summary>
    internal class ClosingSyncLogic : AbstractSyncLogic
    {
        public void Execute(ClosingParameter param)
        {
            DatabaseManager<FileInfoConnection>.Close();
            DatabaseManager<ThumbnailConnection>.Close();
        }
    }
}
