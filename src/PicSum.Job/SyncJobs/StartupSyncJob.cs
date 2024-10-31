using PicSum.Job.SyncLogics;

namespace PicSum.Job.SyncJobs
{
    public sealed class StartupSyncJob
    {
        public void Execute()
        {
            var logic = new StartupSyncLogic();
            logic.Execute();
        }
    }
}
