using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.Job;
using ZLogger;

namespace PicSum.Job.SyncJobs
{

    public sealed class VersionUpTo_12_3_0_0_SyncJob
        : AbstractSyncJob
    {
        public void Execute()
        {
            var logger = LogManager.GetLogger();
            logger.ZLogDebug($"バージョン'12.3.0.0'に更新します。");

#pragma warning disable CA2012
            this.UpdateTDirectoryViewHistoryTable().GetAwaiter().GetResult();
            this.Vacuum().GetAwaiter().GetResult();
#pragma warning restore CA2012

            logger.ZLogDebug($"バージョン'12.3.0.0'に更新しました。");
        }

        private async ValueTask UpdateTDirectoryViewHistoryTable()
        {
            var updateSql = new VersionUpTo_12_3_0_0_Sql();
            await using (var con = await Instance<IFileInfoDao>.Value.ConnectWithTransaction().WithConfig())
            {
                await con.Update(updateSql).WithConfig();
                await con.Commit().WithConfig();
            }
        }

        private async ValueTask Vacuum()
        {
            await using (var con = await Instance<IFileInfoDao>.Value.Connect().WithConfig())
            {
                var cleanupSql = new FileInfoDBVacuumSql();
                await con.ReadLine(cleanupSql).WithConfig();
            }
        }
    }
}
