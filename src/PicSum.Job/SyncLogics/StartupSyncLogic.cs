using PicSum.DatabaseAccessor.Connection;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;

namespace PicSum.Job.SyncLogics
{
    internal sealed class StartupSyncLogic
        : AbstractSyncLogic
    {
        public void Execute()
        {
            Dao<FileInfoDB>.Instance
                .Connect(new FileInfoDB(AppConstants.FILE_INFO_DATABASE_FILE));

            Dao<ThumbnailDB>.Instance
                .Connect(new ThumbnailDB(AppConstants.THUMBNAIL_DATABASE_FILE));
        }
    }
}
