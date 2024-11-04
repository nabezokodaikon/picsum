using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Common;
using SWF.Core.DatabaseAccessor;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;

namespace PicSum.Job.SyncLogics
{
    /// <summary>
    /// 終了同期ロジック
    /// </summary>
    internal sealed class ClosingSyncLogic
        : AbstractSyncLogic
    {
        public void Execute()
        {
            JobCaller.Instance.Dispose();
            FileIconCacher.Instance.Dispose();
            ThumbnailCacher.Instance.Dispose();
            ImageFileCacher.Instance.Dispose();
            ImageFileSizeCacher.Instance.Dispose();
            FileExporter.Instance.Dispose();

            Dao<IFileInfoDB>.Instance.Dispose();
            Dao<IThumbnailDB>.Instance.Dispose();
        }
    }
}
