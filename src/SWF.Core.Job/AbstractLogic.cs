namespace SWF.Core.Job
{
    /// <summary>
    /// 非同期ロジック基底クラス
    /// </summary>
    public abstract class AbstractLogic
    {
        private readonly IJob Job;

        protected AbstractLogic(IJob job)
        {
            ArgumentNullException.ThrowIfNull(job, nameof(job));

            this.Job = job;
        }

        protected void WriteErrorLog(Exception ex)
        {
            this.Job.WriteErrorLog(ex);
        }

        protected void WriteErrorLog(string message)
        {
            this.Job.WriteErrorLog(message);
        }

        protected void ThrowIfJobCancellationRequested()
        {
            this.Job.ThrowIfJobCancellationRequested();
        }
    }
}
