using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Common;
using SWF.Core.Base;
using SWF.Core.ConsoleAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.Job.SyncLogics
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class StartupSyncLogic
        : AbstractSyncLogic
    {
        public void Execute()
        {
            using (TimeMeasuring.Run(true, "StartupSyncLogic.Execute"))
            {
                Instance<IFileInfoDB>.Initialize(() =>
                    new FileInfoDB(AppConstants.FILE_INFO_DATABASE_FILE.Value));

                Instance<IThumbnailDB>.Initialize(() =>
                    new ThumbnailDB(AppConstants.THUMBNAIL_DATABASE_FILE.Value));

                Instance<IFileIconCacher>.Initialize(() => new FileIconCacher());
                Instance<IThumbnailCacher>.Initialize(() => new ThumbnailCacher());
                Instance<IImageFileCacheTasks>.Initialize(() => new ImageFileCacheTasks());
                Instance<IThumbnailCacheTasks>.Initialize(() => new ThumbnailCacheTasks());
                Instance<IImageFileCacher>.Initialize(() => new ImageFileCacher());
                Instance<IImageFileSizeCacher>.Initialize(() => new ImageFileSizeCacher());

                SynchronizationContext.SetSynchronizationContext(
                    new WindowsFormsSynchronizationContext());

                if (SynchronizationContext.Current == null)
                {
                    throw new NullReferenceException("同期コンテキストが生成されていません。");
                }

                Instance<JobCaller>.Initialize(() =>
                    new JobCaller(SynchronizationContext.Current));
            }
        }
    }
}
