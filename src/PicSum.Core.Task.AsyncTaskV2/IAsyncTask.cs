using NLog;

namespace PicSum.Core.Task.AsyncTaskV2
{
    public interface IAsyncTask
    {
        public TaskID? ID { get; set; }
        public void CheckCancel();
        public void WriteErrorLog(TaskException ex);
    }
}
