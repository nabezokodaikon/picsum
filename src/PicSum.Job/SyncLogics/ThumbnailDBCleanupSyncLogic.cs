using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;

namespace PicSum.Job.SyncLogics
{
    internal sealed class ThumbnailDBCleanupSyncLogic
         : AbstractSyncLogic
    {
        public void Execute()
        {
            if (FileUtil.CanAccess(AppConstants.THUMBNAIL_DATABASE_FILE))
            {
                File.Delete(AppConstants.THUMBNAIL_DATABASE_FILE);
            }

            foreach (var thumbnailFile in FileUtil.GetFiles(AppConstants.DATABASE_DIRECTORY)
                .Where(file => FileUtil.GetExtension(file) == AppConstants.THUMBNAIL_BUFFER_FILE_EXTENSION.ToUpper()))
            {
                File.Delete($"{thumbnailFile}");
            }
        }
    }
}
