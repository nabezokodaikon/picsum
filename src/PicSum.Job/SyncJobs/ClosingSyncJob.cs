using PicSum.Job.SyncLogics;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.SyncJobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ClosingSyncJob
        : AbstractSyncJob
    {
        public async ValueTask Execute()
        {
            var logic = new ClosingSyncLogic();
            await logic.Execute();
        }
    }
}
