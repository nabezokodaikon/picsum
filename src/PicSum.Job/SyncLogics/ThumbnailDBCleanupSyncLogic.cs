using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;
using SWF.Core.StringAccessor;
using System.Runtime.Versioning;

namespace PicSum.Job.SyncLogics
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class ThumbnailDBCleanupSyncLogic
         : AbstractSyncLogic
    {
        public void Execute()
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
                .Where(file =>
                StringUtil.CompareFilePath(
                    FileUtil.GetExtensionFastStack(file),
                    ThumbnailUtil.THUMBNAIL_BUFFER_FILE_EXTENSION)))
            {
                File.Delete($"{thumbnailFile}");
            }
        }
    }
}
