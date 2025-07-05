namespace SWF.Core.Job
{
    public interface IAsyncJob
    {
        public bool IsJobCancel { get; }
        public void ThrowIfJobCancellationRequested();
        public void WriteErrorLog(Exception ex);
        public void WriteErrorLog(string message);
        public string ToString();
    }
}
