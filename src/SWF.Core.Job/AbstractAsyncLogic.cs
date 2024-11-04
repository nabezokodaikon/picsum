namespace SWF.Core.Job
{
    /// <summary>
    /// 非同期ロジック基底クラス
    /// </summary>
    public abstract class AbstractAsyncLogic
    {
        protected readonly IAsyncJob Job;

        public AbstractAsyncLogic(IAsyncJob job)
        {
            ArgumentNullException.ThrowIfNull(job, nameof(job));

            this.Job = job;
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
