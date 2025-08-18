using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;
using SWF.Core.StringAccessor;

namespace PicSum.Job.SyncJobs
{

    public sealed class ThumbnailDBCleanupSyncJob
        : AbstractSyncJob
    {
        private const string ERROR_MESSAGE = "サムネイルデータベースクリーンアップジョブで例外が発生しました。";

        public void Execute()
        {
            using (TimeMeasuring.Run(true, "ThumbnailDBCleanupSyncJob.Execute"))
            {
                try
                {
                    var dbFile = AppFiles.THUMBNAIL_DATABASE_FILE.Value;
                    if (FileUtil.CanAccess(dbFile))
                    {
                        File.Delete(dbFile);
                    }

                    var cacheFile = AppFiles.THUMBNAIL_CACHE_FILE.Value;
                    if (FileUtil.CanAccess(cacheFile))
                    {
                        File.Delete(cacheFile);
                    }

                    foreach (var thumbnailFile in FileUtil.GetFiles(AppFiles.DATABASE_DIRECTORY.Value)
                        .Where(static file =>
                        StringUtil.CompareFilePath(
                            FileUtil.GetExtensionFastStack(file),
                            ThumbnailUtil.THUMBNAIL_BUFFER_FILE_EXTENSION)))
                    {
                        File.Delete($"{thumbnailFile}");
                    }
                }
                catch (Exception ex) when (
                    ex is DirectoryNotFoundException ||
                    ex is PathTooLongException ||
                    ex is IOException ||
                    ex is NotSupportedException ||
                    ex is UnauthorizedAccessException)
                {
                    Log.GetLogger().Error(ex, ERROR_MESSAGE);
                }
            }
        }
    }
}
