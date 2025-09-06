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
            using (Measuring.Time(true, "FileInfoDBCleanupSyncJob.Execute"))
            {
                this.Cleanup().AsTask().GetAwaiter().GetResult();
                this.Vacuum().AsTask().GetAwaiter().GetResult();
            }
        }

        private async ValueTask Cleanup()
        {
            var readSql = new AllFilesReadSql();

            var con = await Instance<IFileInfoDao>.Value.ConnectWithTransaction().False();
            try
            {
                var fileList = await con.ReadList(readSql).False();
                foreach (var file in fileList)
                {
                    if (!FileUtil.CanAccess(file.FilePath))
                    {
                        var cleanupSql = new FileInfoDBCleanupSql(file.FileID);
                        await con.Update(cleanupSql).False();
                    }
                }
            }
            finally
            {
                await con.DisposeAsync().False();
            }
        }

        private async ValueTask Vacuum()
        {
            var con = await Instance<IFileInfoDao>.Value.Connect().False();
            try
            {
                var cleanupSql = new FileInfoDBVacuumSql();
                await con.ReadLine(cleanupSql).False();
            }
            finally
            {
                await con.DisposeAsync().False();
            }
        }
    }
}
