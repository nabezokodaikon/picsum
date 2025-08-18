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
            var logger = Log.GetLogger();
            logger.Debug("バージョン'12.2.2.0'に更新します。");

            this.UpdateTDirectoryViewHistoryTable();
            this.Vacuum();

            logger.Debug("バージョン'12.2.2.0'に更新しました。");
        }

        private void UpdateTDirectoryViewHistoryTable()
        {
            var updateSql = new VersionUpTo_12_2_2_0_Sql();
            using (var con = Instance<IFileInfoDB>.Value.ConnectWithTransaction())
            {
                con.Update(updateSql);
                con.Commit();
            }
        }

        private void Vacuum()
        {
            using (var con = Instance<IFileInfoDB>.Value.Connect())
            {
                var cleanupSql = new FileInfoDBVacuumSql();
                con.ReadLine(cleanupSql);
            }
        }
    }
}
