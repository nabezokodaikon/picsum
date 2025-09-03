using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.SyncJobs
{

    public sealed class VersionUpTo_12_2_2_0_SyncJob
        : AbstractSyncJob
    {
        public void Execute()
        {
            var logger = NLogManager.GetLogger();
            logger.Debug("バージョン'12.2.2.0'に更新します。");

            this.UpdateTDirectoryViewHistoryTable().Wait();
            this.Vacuum().Wait();

            logger.Debug("バージョン'12.2.2.0'に更新しました。");
        }

        private async Task UpdateTDirectoryViewHistoryTable()
        {
            var updateSql = new VersionUpTo_12_2_2_0_Sql();
            await using (var con = await Instance<IFileInfoDao>.Value.ConnectWithTransaction().WithConfig())
            {
                await con.Update(updateSql).WithConfig();
                await con.Commit().WithConfig();
            }
        }

        private async Task Vacuum()
        {
            await using (var con = await Instance<IFileInfoDao>.Value.Connect().WithConfig())
            {
                var cleanupSql = new FileInfoDBVacuumSql();
                await con.ReadLine(cleanupSql).WithConfig();
            }
        }
    }
}
