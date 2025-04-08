using PicSum.Job.Parameters;
using PicSum.Job.Results;

namespace PicSum.Job.Common
{
    public interface IThumbnailReadThreads
        : IDisposable
    {
        public void DoCache(
            ThumbnailsGetParameter parameter,
            Action<ThumbnailImageResult> callbackAction);
    }
}
