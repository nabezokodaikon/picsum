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
            using (var tran = Instance<IFileInfoDB>.Value.BeginTransaction())
            {
                this.Cleanup();
                tran.Commit();
            }

            this.Vacuum();
        }

        private void Cleanup()
        {
            var readSql = new AllFilesReadSql();
            var fileList = Instance<IFileInfoDB>.Value
                .ReadList(readSql);
            foreach (var file in fileList)
            {
                if (!FileUtil.IsExists(file.FilePath))
                {
                    var cleanupSql = new FileInfoDBCleanupSql(file.FileID);
                    Instance<IFileInfoDB>.Value.Update(cleanupSql);
                }
            }
        }

        private void Vacuum()
        {
            var cleanupSql = new FileInfoDBVacuumSql();
            Instance<IFileInfoDB>.Value.ReadLine(cleanupSql);
        }
    }
}
