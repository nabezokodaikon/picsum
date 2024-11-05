using PicSum.Job.SyncLogics;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.SyncJobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ThumbnailDBCleanupSyncJob
        : AbstractSyncJob
    {
        private const string ERROR_MESSAGE = "サムネイルデータベースクリーンアップジョブで例外が発生しました。";

        public void Execute()
        {
            try
            {
                var thumbnailLogic = new ThumbnailDBCleanupSyncLogic();
                thumbnailLogic.Execute();
            }
            catch (DirectoryNotFoundException ex)
            {
                Logger.Error(ex, ERROR_MESSAGE);
            }
            catch (PathTooLongException ex)
            {
                Logger.Error(ex, ERROR_MESSAGE);
            }
            catch (IOException ex)
            {
                Logger.Error(ex, ERROR_MESSAGE);
            }
            catch (NotSupportedException ex)
            {
                Logger.Error(ex, ERROR_MESSAGE);
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Error(ex, ERROR_MESSAGE);
            }
        }
    }
}
