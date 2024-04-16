using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Job.AsyncJob;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using SWF.Common;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    [SupportedOSPlatform("windows")]
    internal sealed class ThumbnailDBCleanupLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
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
            var readSql = new AllThumbnailsReadSql();
            var fileList = DatabaseManager<ThumbnailConnection>
                .ReadList(readSql);
            foreach (var file in fileList)
            {
                if (!FileUtil.IsExists(file.FilePath))
                {
                    var cleanupSql = new ThumbnailDBCleanupSql(file.FilePath);
                    DatabaseManager<ThumbnailConnection>.Update(cleanupSql);
                }

            }
        }

        private void Vacuum()
        {
            var cleanupSql = new ThumbnailDBVacuumSql();
            DatabaseManager<ThumbnailConnection>.ReadLine(cleanupSql);
        }
    }
}
