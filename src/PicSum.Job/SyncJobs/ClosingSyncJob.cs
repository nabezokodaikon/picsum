using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Common;
using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;

namespace PicSum.Job.SyncJobs
{

    public sealed class ClosingSyncJob
        : AbstractSyncJob
    {
        public void Execute()
        {
            using (Measuring.Time(true, "ClosingSyncJob.Execute"))
            {
                Instance<JobCaller>.Value.Dispose();
                Instance<IFileIconCacher>.Value.Dispose();
                Instance<IThumbnailCacher>.Value.Dispose();
                Instance<IImageFileCacher>.Value.Dispose();
                Instance<IImageFileSizeCacher>.Value.Dispose();
                Instance<IImageFileTakenDateCacher>.Value.Dispose();
                Instance<IFileInfoDao>.Value.Dispose();
                Instance<IThumbnailDao>.Value.Dispose();
            }
        }
    }
}
