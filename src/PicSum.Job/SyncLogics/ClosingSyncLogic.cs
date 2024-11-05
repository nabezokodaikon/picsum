using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Common;
using SWF.Core.Base;
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
            Instance<JobCaller>.Value.Dispose();
            Instance<IFileIconCacher>.Value.Dispose();
            Instance<IThumbnailCacher>.Value.Dispose();
            Instance<IImageFileCacher>.Value.Dispose();
            Instance<IImageFileSizeCacher>.Value.Dispose();
            Instance<IFileExporter>.Value.Dispose();
            Instance<IFileInfoDB>.Value.Dispose();
            Instance<IThumbnailDB>.Value.Dispose();
        }
    }
}
