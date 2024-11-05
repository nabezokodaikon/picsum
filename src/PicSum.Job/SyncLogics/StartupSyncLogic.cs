using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Common;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;

namespace PicSum.Job.SyncLogics
{
    internal sealed class StartupSyncLogic
        : AbstractSyncLogic
    {
        public void Execute()
        {
            Instance<IFileInfoDB>.Initialize(
                new FileInfoDB(AppConstants.FILE_INFO_DATABASE_FILE));

            Instance<IThumbnailDB>.Initialize(
                new ThumbnailDB(AppConstants.THUMBNAIL_DATABASE_FILE));

            Instance<IFileIconCacher>.Initialize(new FileIconCacher());
            Instance<IThumbnailCacher>.Initialize(new ThumbnailCacher());
            Instance<IImageFileCacher>.Initialize(new ImageFileCacher());
            Instance<IImageFileSizeCacher>.Initialize(new ImageFileSizeCacher());
            Instance<IFileExporter>.Initialize(new FileExporter());
        }
    }
}
