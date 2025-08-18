using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.SyncJobs
{

    public sealed class VersionUpTo_12_2_1_0_SyncJob
        : AbstractSyncJob
    {
        public void Execute()
        {
            var logger = Log.GetLogger();
            logger.Debug("バージョン'12.2.1.0'に更新します。");

            this.UpdateTRatingTable();
            this.Vacuum();

            logger.Debug("バージョン'12.2.1.0'に更新しました。");
        }

        private void UpdateTRatingTable()
        {
            var updateSql = new VersionUpTo_12_2_1_0_Sql();
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
