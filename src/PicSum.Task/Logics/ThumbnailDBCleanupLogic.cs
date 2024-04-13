using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using SWF.Common;
using System.Runtime.Versioning;

namespace PicSum.Task.Logics
{
    [SupportedOSPlatform("windows")]
    internal sealed class ThumbnailDBCleanupLogic(IAsyncTask task)
        : AbstractAsyncLogic(task)
    {
        public void Execute()
        {
            using (var tran = DatabaseManager<ThumbnailConnection>.BeginTransaction())
            {
                this.Cleanup();
                tran.Commit();
            }

            this.Vacuum();
        }

        private void Cleanup()
        {
            var readSql = new ReadAllThumbnailSql();
            var fileList = DatabaseManager<ThumbnailConnection>
                .ReadList(readSql);
            foreach (var file in fileList)
            {
                if (!FileUtil.IsExists(file.FilePath))
                {
                    var cleanupSql = new CleanupThumbnailSql(file.FilePath);
                    DatabaseManager<ThumbnailConnection>.Update(cleanupSql);
                }

            }
        }

        private void Vacuum()
        {
            var cleanupSql = new VacuumThumbnailSql();
            DatabaseManager<ThumbnailConnection>.ReadLine(cleanupSql);
        }
    }
}
