using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.SyncJobs
{

    public sealed class VersionUpTo_17_1_0_0_SyncJob
        : AbstractSyncJob
    {
        public void Execute()
        {
            var logger = NLogManager.GetLogger();
            logger.Debug("バージョン'17.1.0.0'に更新します。");

            var logic = new ThumbnailDBCleanupSyncJob();
            logic.Execute();

            logger.Debug("バージョン'17.1.0.0'に更新しました。");
        }
    }
}
