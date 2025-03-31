using PicSum.Job.SyncLogics;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.SyncJobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class StartupSyncJob
        : AbstractSyncJob
    {
        public void Execute()
        {
            ConsoleUtil.Write($"StartupSyncJob.Execute Start");

            var logic = new StartupSyncLogic();
            logic.Execute();

            ConsoleUtil.Write($"StartupSyncJob.Execute End");
        }
    }
}
