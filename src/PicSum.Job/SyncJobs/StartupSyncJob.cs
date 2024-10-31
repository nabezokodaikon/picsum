using PicSum.Job.SyncLogics;
using SWF.Core.Job;

namespace PicSum.Job.SyncJobs
{
    public sealed class StartupSyncJob
        : AbstractSyncJob
    {
        public void Execute()
        {
            var logic = new StartupSyncLogic();
            logic.Execute();
        }
    }
}
