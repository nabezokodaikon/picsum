using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.SyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.Entity;

namespace PicSum.Task.SyncLogic
{
    /// <summary>
    /// 終了同期ロジック
    /// </summary>
    internal class ClosingSyncLogic : SyncLogicBase
    {
        public void Execute(ClosingParameterEntity param)
        {
            DatabaseManager<FileInfoConnection>.Close();
            DatabaseManager<ThumbnailConnection>.Close();
        }
    }
}
