using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.SyncJobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class FileInfoDBCleanupSyncJob
        : AbstractSyncJob
    {
        public async ValueTask Execute()
        {
            using (TimeMeasuring.Run(true, "FileInfoDBCleanupSyncJob.Execute"))
            {
                await this.Cleanup();
                await this.Vacuum();
            }
        }

        private async ValueTask Cleanup()
        {
            var readSql = new AllFilesReadSql();
            await using (var con = await Instance<IFileInfoDB>.Value.ConnectWithTransaction())
            {
                var fileList = con.ReadList(readSql);
                foreach (var file in fileList)
                {
                    if (!FileUtil.CanAccess(file.FilePath))
                    {
                        var cleanupSql = new FileInfoDBCleanupSql(file.FileID);
                        con.Update(cleanupSql);
                    }
                }

                con.Commit();
            }
        }

        private async ValueTask Vacuum()
        {
            await using (var con = await Instance<IFileInfoDB>.Value.Connect())
            {
                var cleanupSql = new FileInfoDBVacuumSql();
                con.ReadLine(cleanupSql);
            }
        }
    }
}
