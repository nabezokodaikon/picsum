namespace SWF.Core.Job
{
    public interface IAsyncJob
    {
        public JobID ID { get; }
        public void CheckCancel();
        public void WriteErrorLog(JobException ex);
    }
}
