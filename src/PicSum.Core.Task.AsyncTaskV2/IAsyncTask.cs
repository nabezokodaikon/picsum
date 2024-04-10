using NLog;

namespace PicSum.Core.Task.AsyncTaskV2
{
    public interface IAsyncTask
    {
        public void CheckCancel();
        public void WriteErrorLog(Exception ex);
    }
}
