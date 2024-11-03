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
            DatabaseManager<FileInfoConnection>
                .Connect(new FileInfoConnection(ResourceUtil.FILE_INFO_DATABASE_FILE));

            DatabaseManager<ThumbnailConnection>
                .Connect(new ThumbnailConnection(ResourceUtil.THUMBNAIL_DATABASE_FILE));
        }
    }
}
