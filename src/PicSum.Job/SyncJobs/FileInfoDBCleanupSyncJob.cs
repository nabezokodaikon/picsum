using PicSum.Job.SyncLogics;
using SWF.Core.ConsoleAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.SyncJobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class FileInfoDBCleanupSyncJob
        : AbstractSyncJob
    {
        public void Execute()
        {
            var logger = Log.GetLogger();
            logger.Debug("ファイル情報データベースクリーンアップジョブを開始します。");

            try
            {
                var fileInfoLogic = new FileInfoDBCleanupSyncLogic();
                fileInfoLogic.Execute();
            }
            finally
            {
                logger.Debug("ファイル情報データベースクリーンアップジョブが終了しました。");
            }
        }
    }
}
