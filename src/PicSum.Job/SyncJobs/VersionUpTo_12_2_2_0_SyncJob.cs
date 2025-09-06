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

            this.UpdateTDirectoryViewHistoryTable().AsTask().GetAwaiter().GetResult();
            this.Vacuum().AsTask().GetAwaiter().GetResult();

            logger.Debug("バージョン'12.2.2.0'に更新しました。");
        }

        private async ValueTask UpdateTDirectoryViewHistoryTable()
        {
            var updateSql = new VersionUpTo_12_2_2_0_Sql();

            var con = await Instance<IFileInfoDao>.Value.ConnectWithTransaction().False();
            try
            {
                await con.Update(updateSql).False();
                await con.Commit().False();
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
