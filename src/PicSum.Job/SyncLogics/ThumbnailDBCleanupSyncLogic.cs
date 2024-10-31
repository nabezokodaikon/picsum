using SWF.Core.FileAccessor;
using SWF.Core.Job;

namespace PicSum.Job.SyncLogics
{
    internal sealed class ThumbnailDBCleanupSyncLogic
         : AbstractSyncLogic
    {
        public void Execute()
        {
            var dbFile = Path.Combine(FileUtil.DATABASE_DIRECTORY, "thumbnail.sqlite");
            if (FileUtil.CanAccess(dbFile))
            {
                File.Delete(dbFile);
            }

            foreach (var thumbnailFile in FileUtil.GetFiles(FileUtil.DATABASE_DIRECTORY)
                .Where(file => FileUtil.GetExtension(file) == ".THUMBNAIL"))
            {
                File.Delete($"{thumbnailFile}");
            }
        }
    }
}
