using PicSum.Job.SyncLogics;
using SWF.Core.ConsoleAccessor;
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
            var logger = Log.GetLogger();
            logger.Debug("サムネイルデータベースクリーンアップジョブを開始します。");

            try
            {
                var thumbnailLogic = new ThumbnailDBCleanupSyncLogic();
                thumbnailLogic.Execute();
            }
            catch (DirectoryNotFoundException ex)
            {
                logger.Error(ex, ERROR_MESSAGE);
            }
            catch (PathTooLongException ex)
            {
                logger.Error(ex, ERROR_MESSAGE);
            }
            catch (IOException ex)
            {
                logger.Error(ex, ERROR_MESSAGE);
            }
            catch (NotSupportedException ex)
            {
                logger.Error(ex, ERROR_MESSAGE);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.Error(ex, ERROR_MESSAGE);
            }
            finally
            {
                logger.Debug("サムネイルデータベースクリーンアップジョブが終了しました。");
            }
        }
    }
}
