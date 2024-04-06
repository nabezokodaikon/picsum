namespace PicSum.Core.Task.AsyncTaskV2
{
    /// <summary>
    /// 非同期ロジック基底クラス
    /// </summary>
    public abstract class AbstractAsyncLogic
    {
        private readonly IAsyncTask task;

        public void CheckCancel()
        {
            this.task.CheckCancel();
        }

        public AbstractAsyncLogic(IAsyncTask task)
        {
            this.task = task ?? throw new ArgumentNullException("task");
        }
    }
}
