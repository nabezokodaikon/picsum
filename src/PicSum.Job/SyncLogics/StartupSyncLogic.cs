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
                .Connect(new FileInfoConnection(Path.Combine(FileUtil.DATABASE_DIRECTORY, @"fileinfo.sqlite")));

            DatabaseManager<ThumbnailConnection>
                .Connect(new ThumbnailConnection(Path.Combine(FileUtil.DATABASE_DIRECTORY, @"thumbnail.sqlite")));
        }
    }
}
