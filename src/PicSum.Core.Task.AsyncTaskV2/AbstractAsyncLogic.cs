namespace PicSum.Core.Task.AsyncTaskV2
{
    /// <summary>
    /// 非同期ロジック基底クラス
    /// </summary>
    public abstract class AbstractAsyncLogic(IAsyncTask task)
    {
        private readonly IAsyncTask task
            = task ?? throw new ArgumentNullException("task");

        protected TaskID? ID { get; private set; } = task.ID;

        protected void WriteErrorLog(TaskException ex)
        {
            task.WriteErrorLog(ex);
        }

        protected void CheckCancel()
        {
            this.task.CheckCancel();
        }
    }
}
