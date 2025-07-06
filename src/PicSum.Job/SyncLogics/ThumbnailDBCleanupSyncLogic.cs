using SWF.Core.App;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
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
            if (FileUtil.CanAccess(AppConstants.THUMBNAIL_DATABASE_FILE.Value))
            {
                File.Delete(AppConstants.THUMBNAIL_DATABASE_FILE.Value);
            }

            foreach (var thumbnailFile in FileUtil.GetFiles(AppConstants.DATABASE_DIRECTORY.Value)
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
