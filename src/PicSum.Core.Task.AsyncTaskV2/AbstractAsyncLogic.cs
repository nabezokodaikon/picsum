namespace PicSum.Core.Task.AsyncTaskV2
{
    /// <summary>
    /// 非同期ロジック基底クラス
    /// </summary>
    public abstract class AbstractAsyncLogic
    {
        private readonly IAsyncTask task;

        public AbstractAsyncLogic(IAsyncTask task)
        {
            this.task = task ?? throw new ArgumentNullException("task");
        }

        protected void WriteErrorLog(Exception ex)
        {
            task.WriteErrorLog(ex);
        }

        protected void CheckCancel()
        {
            this.task.CheckCancel();
        }
    }
}
