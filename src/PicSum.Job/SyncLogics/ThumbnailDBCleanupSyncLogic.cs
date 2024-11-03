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
            if (FileUtil.CanAccess(ResourceUtil.THUMBNAIL_DATABASE_FILE))
            {
                File.Delete(ResourceUtil.THUMBNAIL_DATABASE_FILE);
            }

            foreach (var thumbnailFile in FileUtil.GetFiles(ResourceUtil.DATABASE_DIRECTORY)
                .Where(file => FileUtil.GetExtension(file) == ResourceUtil.THUMBNAIL_BUFFER_FILE_EXTENSION.ToUpper()))
            {
                File.Delete($"{thumbnailFile}");
            }
        }
    }
}
