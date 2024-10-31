using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.FileAccessor;

namespace PicSum.Job.SyncLogics
{
    internal sealed class FileInfoDBCleanupSyncLogic
    {
        public void Execute()
        {
            using (var tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                this.Cleanup();
                tran.Commit();
            }

            this.Vacuum();
        }

        private void Cleanup()
        {
            var readSql = new AllFilesReadSql();
            var fileList = DatabaseManager<FileInfoConnection>
                .ReadList(readSql);
            foreach (var file in fileList)
            {
                if (!FileUtil.IsExists(file.FilePath))
                {
                    var cleanupSql = new FileInfoDBCleanupSql(file.FileID);
                    DatabaseManager<FileInfoConnection>.Update(cleanupSql);
                }
            }
        }

        private void Vacuum()
        {
            var cleanupSql = new FileInfoDBVacuumSql();
            DatabaseManager<FileInfoConnection>.ReadLine(cleanupSql);
        }
    }
}
