using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Common;
using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;
using System.Runtime.Versioning;

namespace PicSum.Job.SyncLogics
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class ClosingSyncLogic
        : AbstractSyncLogic
    {
        public async ValueTask Execute()
        {
            using (TimeMeasuring.Run(true, "ClosingSyncLogic.Execute"))
            {
                await Instance<JobCaller>.Value.DisposeAsync();
                Instance<IFileIconCacher>.Value.Dispose();
                Instance<IThumbnailCacher>.Value.Dispose();
                Instance<IImageFileCacher>.Value.Dispose();
                Instance<IImageFileSizeCacher>.Value.Dispose();
                Instance<IFileInfoDB>.Value.Dispose();
                Instance<IThumbnailDB>.Value.Dispose();
            }
        }
    }
}
