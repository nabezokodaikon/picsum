using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Common;
using SWF.Core.Base;
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
                Instance<IFileInfoDB>.Initialize(new Lazy<IFileInfoDB>(
                    () => new FileInfoDB(AppFiles.FILE_INFO_DATABASE_FILE.Value),
                    LazyThreadSafetyMode.ExecutionAndPublication));

                Instance<IThumbnailDB>.Initialize(new Lazy<IThumbnailDB>(
                    () => new ThumbnailDB(AppFiles.THUMBNAIL_DATABASE_FILE.Value),
                    LazyThreadSafetyMode.ExecutionAndPublication));

                Instance<IFileIconCacher>.Initialize(new Lazy<IFileIconCacher>(
                    () => new FileIconCacher(),
                    LazyThreadSafetyMode.ExecutionAndPublication));

                Instance<IThumbnailCacher>.Initialize(new Lazy<IThumbnailCacher>(
                    () => new ThumbnailCacher(),
                    LazyThreadSafetyMode.ExecutionAndPublication));

                Instance<IImageFileCacher>.Initialize(new Lazy<IImageFileCacher>(
                    () => new ImageFileCacher(),
                    LazyThreadSafetyMode.ExecutionAndPublication));

                Instance<IImageFileSizeCacher>.Initialize(new Lazy<IImageFileSizeCacher>(
                    () => new ImageFileSizeCacher(),
                    LazyThreadSafetyMode.ExecutionAndPublication));

                SynchronizationContext.SetSynchronizationContext(
                    new WindowsFormsSynchronizationContext());

                if (SynchronizationContext.Current == null)
                {
                    throw new NullReferenceException("同期コンテキストが生成されていません。");
                }

                Instance<JobCaller>.Initialize(new Lazy<JobCaller>(
                    () => new JobCaller(SynchronizationContext.Current),
                    LazyThreadSafetyMode.ExecutionAndPublication));
            }
        }
    }
}
