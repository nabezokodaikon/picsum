namespace PicSum.Core.Job.AsyncJob
{
    /// <summary>
    /// 非同期ロジック基底クラス
    /// </summary>
    public abstract class AbstractAsyncLogic(IAsyncJob job)
    {
        private readonly IAsyncJob job
            = job ?? throw new ArgumentNullException("job");

        protected JobID? ID { get; private set; } = job.ID;

        protected void WriteErrorLog(JobException ex)
        {
            job.WriteErrorLog(ex);
        }

        protected void CheckCancel()
        {
            this.job.CheckCancel();
        }
    }
}
