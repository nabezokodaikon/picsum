using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Logics;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows")]
    public sealed class DBCleanupJob
        : AbstractOneWayJob
    {
        protected override void Execute()
        {
            var fileInfoLogic = new FileInfoDBCleanupLogic(this);
            fileInfoLogic.Execute();

            var thumbnailLogic = new ThumbnailDBCleanupLogic(this);
            thumbnailLogic.Execute();
        }
    }
}
