using PicSum.Job.SyncLogics;
using SWF.Core.Job;

namespace PicSum.Job.SyncJobs
{
    public sealed class FileInfoDBCleanupSyncJob
        : AbstractSyncJob
    {
        public void Execute()
        {
            var fileInfoLogic = new FileInfoDBCleanupSyncLogic();
            fileInfoLogic.Execute();
        }
    }
}
