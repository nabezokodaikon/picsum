using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.SyncJobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class VersionUpTo_12_2_1_0_Job
        : AbstractSyncJob
    {
        public async ValueTask Execute()
        {
            var logger = Log.GetLogger();
            logger.Debug("バージョン'12.2.1.0'に更新します。");

            await this.UpdateTRatingTable();
            await this.Vacuum();

            logger.Debug("バージョン'12.2.1.0'に更新しました。");
        }

        private async ValueTask UpdateTRatingTable()
        {
            var updateSql = new VersionUpTo_12_2_1_0_Sql();
            await using (var con = await Instance<IFileInfoDB>.Value.ConnectWithTransaction())
            {
                con.Update(updateSql);
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
