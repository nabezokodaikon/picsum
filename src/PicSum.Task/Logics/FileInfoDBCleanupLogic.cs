using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using SWF.Common;
using System.Runtime.Versioning;

namespace PicSum.Task.Logics
{
    [SupportedOSPlatform("windows")]
    internal sealed class FileInfoDBCleanupLogic(IAsyncTask task)
        : AbstractAsyncLogic(task)
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
