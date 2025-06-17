using PicSum.Job.Parameters;
using PicSum.Job.Results;

namespace PicSum.Job.Common
{
    public interface IThumbnailCacheTasks
        : IDisposable
    {
        public void DoCache(
            ThumbnailsGetParameter parameter,
            Action<ThumbnailImageResult> callbackAction);
    }
}
