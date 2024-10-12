using PicSum.Core.Job.AsyncJob;
using PicSum.Job.SyncLogics;

namespace PicSum.Job.SyncJobs
{
    /// <summary>
    /// 終了同期ジョブ
    /// </summary>
    public sealed class ClosingSyncJob
        : AbstractSyncJob
    {
        public void Execute()
        {
            var logic = new ClosingSyncLogic();
            logic.Execute();
        }
    }
}
