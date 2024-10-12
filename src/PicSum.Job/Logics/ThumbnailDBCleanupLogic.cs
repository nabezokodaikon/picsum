using SWF.Core.Job;
using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    [SupportedOSPlatform("windows")]
    internal sealed class ThumbnailDBCleanupLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public void Execute(string dbDir)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(dbDir, nameof(dbDir));

            var dbFile = Path.Combine(dbDir, "thumbnail.sqlite");
            if (FileUtil.CanAccess(dbFile))
            {
                File.Delete(dbFile);
            }

            foreach (var thumbnailFile in FileUtil.GetFiles(dbDir)
                .Where(file => FileUtil.GetExtension(file) == ".THUMBNAIL"))
            {
                File.Delete($"{thumbnailFile}");
            }
        }
    }
}
