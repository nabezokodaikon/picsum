using NLog;

namespace PicSum.Core.Job.AsyncJob
{
    public interface IAsyncJob
    {
        public JobID? ID { get; set; }
        public void CheckCancel();
        public void WriteErrorLog(JobException ex);
    }
}
