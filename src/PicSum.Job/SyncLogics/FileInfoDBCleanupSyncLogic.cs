using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
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
            using (var con = Instance<IFileInfoDB>.Value.Connect())
            {
                this.Cleanup(con);
                this.Vacuum(con);
            }
        }

        private void Cleanup(IConnection con)
        {
            var readSql = new AllFilesReadSql();
            var fileList = con.ReadList(readSql);
            foreach (var file in fileList)
            {
                if (!FileUtil.CanAccess(file.FilePath))
                {
                    var cleanupSql = new FileInfoDBCleanupSql(file.FileID);
                    con.Update(cleanupSql);
                }
            }
        }

        private void Vacuum(IConnection con)
        {
            var cleanupSql = new FileInfoDBVacuumSql();
            con.ReadLine(cleanupSql);
        }
    }
}
