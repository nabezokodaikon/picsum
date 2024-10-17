namespace SWF.Core.Job
{
    /// <summary>
    /// 非同期ロジック基底クラス
    /// </summary>
    public abstract class AbstractAsyncLogic
    {
        private readonly AbstractAsyncJob job;

        protected JobID ID { get; private set; }

        public AbstractAsyncLogic(AbstractAsyncJob job)
        {
            ArgumentNullException.ThrowIfNull(job, nameof(job));

            this.job = job;
            this.ID = job.ID;
        }

        protected void WriteErrorLog(JobException ex)
        {
            this.job.WriteErrorLog(ex);
        }

        protected void CheckCancel()
        {
            this.job.CheckCancel();
        }
    }
}
