using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Logics;
using System.Runtime.Versioning;

namespace PicSum.Task.Tasks
{
    [SupportedOSPlatform("windows")]
    public sealed class DBCleanupTask
        : AbstractOneWayTask
    {
        protected override void Execute()
        {
            var fileInfoLogic = new CleanupFileInfoDBLogic(this);
            fileInfoLogic.Execute();

            var thumbnailLogic = new CleanupThumbnailDBLogic(this);
            thumbnailLogic.Execute();
        }
    }
}
