using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;

namespace PicSum.Job.SyncJobs
{

    public sealed class FileInfoDBCleanupSyncJob
        : AbstractSyncJob
    {
        public void Execute()
        {
            using (TimeMeasuring.Run(true, "FileInfoDBCleanupSyncJob.Execute"))
            {
#pragma warning disable CA2012
                this.Cleanup().GetAwaiter().GetResult();
                this.Vacuum().GetAwaiter().GetResult();
#pragma warning restore CA2012
            }
        }

        private async ValueTask Cleanup()
        {
            var readSql = new AllFilesReadSql();
            var con = await Instance<IFileInfoDB>.Value.ConnectWithTransaction().ConfigureAwait(false);
            await using (con)
            {
                var fileList = await con.ReadList(readSql).WithConfig();
                foreach (var file in fileList)
                {
                    if (!FileUtil.CanAccess(file.FilePath))
                    {
                        var cleanupSql = new FileInfoDBCleanupSql(file.FileID);
                        await con.Update(cleanupSql).WithConfig();
                    }
                }

                await con.Commit().WithConfig();
            }
        }

        private async ValueTask Vacuum()
        {
            var con = await Instance<IFileInfoDB>.Value.Connect().ConfigureAwait(false);
            await using (con)
            {
                var cleanupSql = new FileInfoDBVacuumSql();
                await con.ReadLine(cleanupSql).WithConfig();
            }
        }
    }
}
