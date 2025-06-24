using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.SyncLogics
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class FileInfoDBCleanupSyncLogic
        : AbstractSyncLogic
    {
        public void Execute()
        {
            this.Cleanup();
            this.Vacuum();
        }

        private void Cleanup()
        {
            var readSql = new AllFilesReadSql();
            using (var con = Instance<IFileInfoDB>.Value.ConnectWithTransaction())
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
