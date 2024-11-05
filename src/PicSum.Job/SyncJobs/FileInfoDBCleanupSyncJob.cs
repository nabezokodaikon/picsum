using PicSum.Job.SyncLogics;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.SyncJobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
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
