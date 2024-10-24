using PicSum.Job.Logics;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows")]
    public sealed class FileInfoDBCleanupJob
        : AbstractOneWayJob
    {
        protected override void Execute()
        {
            var fileInfoLogic = new FileInfoDBCleanupLogic(this);
            fileInfoLogic.Execute();
        }
    }
}
