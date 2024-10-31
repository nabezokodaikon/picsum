using SWF.Core.FileAccessor;
using SWF.Core.Job;

namespace PicSum.Job.SyncLogics
{
    internal sealed class ThumbnailDBCleanupSyncLogic
         : AbstractSyncLogic
    {
        public void Execute()
        {
            if (FileUtil.CanAccess(FileUtil.THUMBNAIL_DATABASE_FILE))
            {
                File.Delete(FileUtil.THUMBNAIL_DATABASE_FILE);
            }

            foreach (var thumbnailFile in FileUtil.GetFiles(FileUtil.DATABASE_DIRECTORY)
                .Where(file => FileUtil.GetExtension(file) == FileUtil.THUMBNAIL_BUFFER_FILE_EXTENSION.ToUpper()))
            {
                File.Delete($"{thumbnailFile}");
            }
        }
    }
}
