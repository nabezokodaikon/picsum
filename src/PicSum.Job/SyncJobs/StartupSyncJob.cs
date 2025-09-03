using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Common;
using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;

namespace PicSum.Job.SyncJobs
{

    public sealed class StartupSyncJob
        : AbstractSyncJob
    {
        public void Execute()
        {
            using (TimeMeasuring.Run(true, "StartupSyncJob.Execute"))
            {
                Instance<IFileInfoDao>.Initialize(new Lazy<IFileInfoDao>(
                    static () => new FileInfoDao(AppFiles.FILE_INFO_DATABASE_FILE.Value),
                    LazyThreadSafetyMode.ExecutionAndPublication));

                Instance<IThumbnailDao>.Initialize(new Lazy<IThumbnailDao>(
                    static () => new ThumbnailDao(AppFiles.THUMBNAIL_DATABASE_FILE.Value),
                    LazyThreadSafetyMode.ExecutionAndPublication));

                Instance<IFileIconCacher>.Initialize(new Lazy<IFileIconCacher>(
                    static () => new FileIconCacher(),
                    LazyThreadSafetyMode.ExecutionAndPublication));

                Instance<IThumbnailCacher>.Initialize(new Lazy<IThumbnailCacher>(
                    static () =>
                    {
                        var instance = new ThumbnailCacher();
                        instance.Initialize().Wait();
                        return instance;
                    },
                    LazyThreadSafetyMode.ExecutionAndPublication));

                Instance<IImageFileCacher>.Initialize(new Lazy<IImageFileCacher>(
                    static () => new ImageFileCacher(),
                    LazyThreadSafetyMode.ExecutionAndPublication));

                Instance<IImageFileSizeCacher>.Initialize(new Lazy<IImageFileSizeCacher>(
                    static () => new ImageFileSizeCacher(),
                    LazyThreadSafetyMode.ExecutionAndPublication));

                Instance<IImageFileTakenDateCacher>.Initialize(new Lazy<IImageFileTakenDateCacher>(
                    static () => new ImageFileTakenDateCacher(),
                    LazyThreadSafetyMode.ExecutionAndPublication));

                Instance<JobCaller>.Initialize(new Lazy<JobCaller>(
                    static () => new JobCaller(),
                    LazyThreadSafetyMode.ExecutionAndPublication));
            }
        }
    }
}
