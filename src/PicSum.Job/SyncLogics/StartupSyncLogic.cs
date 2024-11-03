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
                .Connect(new FileInfoDB(ResourceUtil.FILE_INFO_DATABASE_FILE));

            Dao<ThumbnailDB>.Instance
                .Connect(new ThumbnailDB(ResourceUtil.THUMBNAIL_DATABASE_FILE));
        }
    }
}
