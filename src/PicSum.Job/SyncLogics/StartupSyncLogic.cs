using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Common;
using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.SyncLogics
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class StartupSyncLogic
        : AbstractSyncLogic
    {
        public void Execute()
        {
            using (TimeMeasuring.Run(false, "StartupSyncLogic.Execute"))
            {
                Instance<IFileInfoDB>.Initialize(
                    new FileInfoDB(AppConstants.FILE_INFO_DATABASE_FILE.Value));

                Instance<IThumbnailDB>.Initialize(
                    new ThumbnailDB(AppConstants.THUMBNAIL_DATABASE_FILE.Value));

                Instance<IFileIconCacher>.Initialize(new FileIconCacher());
                Instance<IThumbnailCacher>.Initialize(new ThumbnailCacher());
                Instance<IImageFileCacheThreads>.Initialize(new ImageFileCacheThreads());
                Instance<IImageFileCacher>.Initialize(new ImageFileCacher());
                Instance<IImageFileSizeCacher>.Initialize(new ImageFileSizeCacher());
            }
        }
    }
}
