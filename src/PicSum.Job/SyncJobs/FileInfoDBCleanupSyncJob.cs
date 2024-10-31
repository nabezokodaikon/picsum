using PicSum.Job.SyncLogics;

namespace PicSum.Job.SyncJobs
{
    public sealed class FileInfoDBCleanupSyncJob
    {
        public void Execute()
        {
            var fileInfoLogic = new FileInfoDBCleanupSyncLogic();
            fileInfoLogic.Execute();
        }
    }
}
