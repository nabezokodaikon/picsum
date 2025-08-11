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
        public void Execute()
        {
            using (TimeMeasuring.Run(true, "FileInfoDBCleanupSyncJob.Execute"))
            {
                this.Cleanup().GetAwaiter().GetResult();
                this.Vacuum().GetAwaiter().GetResult();
            }
        }

        private async Task Cleanup()
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

        private async Task Vacuum()
        {
            await using (var con = await Instance<IFileInfoDB>.Value.Connect())
            {
                var cleanupSql = new FileInfoDBVacuumSql();
                con.ReadLine(cleanupSql);
            }
        }
    }
}
