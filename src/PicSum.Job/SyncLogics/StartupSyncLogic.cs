using PicSum.DatabaseAccessor.Connection;
using SWF.Core.DatabaseAccessor;
using SWF.Core.FileAccessor;
using SWF.Core.Job;

namespace PicSum.Job.SyncLogics
{
    internal sealed class StartupSyncLogic
        : AbstractSyncLogic
    {
        public void Execute()
        {
            DatabaseManager<FileInfoConnection>
                .Connect(new FileInfoConnection(FileUtil.FILE_INFO_DATABASE_FILE));

            DatabaseManager<ThumbnailConnection>
                .Connect(new ThumbnailConnection(FileUtil.THUMBNAIL_DATABASE_FILE));
        }
    }
}
