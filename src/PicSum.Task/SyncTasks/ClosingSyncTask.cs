using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.SyncLogics;

namespace PicSum.Task.SyncTasks
{
    /// <summary>
    /// 終了同期タスク
    /// </summary>
    public sealed class ClosingSyncTask
        : AbstractSyncTask
    {
        public void Execute()
        {
            var logic = new ClosingSyncLogic();
            logic.Execute();
        }
    }
}
