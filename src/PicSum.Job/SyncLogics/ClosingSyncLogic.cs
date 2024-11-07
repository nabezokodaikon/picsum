using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Common;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.SyncLogics
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class ClosingSyncLogic
        : AbstractSyncLogic
    {
        public void Execute()
        {
            Instance<JobCaller>.Value.Dispose();
            Instance<IFileIconCacher>.Value.Dispose();
            Instance<IThumbnailCacher>.Value.Dispose();
            Instance<IClipFiles>.Value.Dispose();
            Instance<IImageFileCacher>.Value.Dispose();
            Instance<IImageFileSizeCacher>.Value.Dispose();
            Instance<IFileExporter>.Value.Dispose();
            Instance<IFileInfoDB>.Value.Dispose();
            Instance<IThumbnailDB>.Value.Dispose();
        }
    }
}
