using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;
using System.Runtime.Versioning;

namespace PicSum.Job.SyncJobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class VersionUpTo_12_2_1_0_Job
        : AbstractSyncJob
    {
        public void Execute()
        {
            var logger = Log.GetLogger();
            logger.Debug("バージョン'12.2.1.0'に更新します。");

            try
            {
                Instance<IFileInfoDB>.Initialize(() =>
                    new FileInfoDB(AppFiles.FILE_INFO_DATABASE_FILE.Value));

                this.UpdateTRatingTable();
                this.Vacuum();
            }
            finally
            {
                Instance<IFileInfoDB>.Value.Dispose();
                logger.Debug("バージョン'12.2.1.0'に更新しました。");
            }
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
