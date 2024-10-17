namespace SWF.Core.Job
{
    /// <summary>
    /// 非同期ロジック基底クラス
    /// </summary>
    public abstract class AbstractAsyncLogic
    {
        protected readonly AbstractAsyncJob Job;

        protected JobID ID { get; private set; }

        public AbstractAsyncLogic(AbstractAsyncJob job)
        {
            ArgumentNullException.ThrowIfNull(job, nameof(job));

            this.Job = job;
            this.ID = job.ID;
        }

        protected void WriteErrorLog(JobException ex)
        {
            this.Job.WriteErrorLog(ex);
        }

        protected void CheckCancel()
        {
            this.Job.CheckCancel();
        }
    }
}
