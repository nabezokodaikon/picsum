using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.FileAccessor;
using SWF.Core.Job;

namespace PicSum.Job.SyncLogics
{
    internal sealed class FileInfoDBCleanupSyncLogic
        : AbstractSyncLogic
    {
        public void Execute()
        {
            using (var tran = Dao<IFileInfoDB>.Instance.BeginTransaction())
            {
                this.Cleanup();
                tran.Commit();
            }

            this.Vacuum();
        }

        private void Cleanup()
        {
            var readSql = new AllFilesReadSql();
            var fileList = Dao<IFileInfoDB>.Instance
                .ReadList(readSql);
            foreach (var file in fileList)
            {
                if (!FileUtil.IsExists(file.FilePath))
                {
                    var cleanupSql = new FileInfoDBCleanupSql(file.FileID);
                    Dao<IFileInfoDB>.Instance.Update(cleanupSql);
                }
            }
        }

        private void Vacuum()
        {
            var cleanupSql = new FileInfoDBVacuumSql();
            Dao<IFileInfoDB>.Instance.ReadLine(cleanupSql);
        }
    }
}
