using PicSum.Core.Task.Base;

namespace PicSum.Core.Task.AsyncTask
{
    /// <summary>
    /// 非同期ファサード基底クラス
    /// </summary>
    public abstract class AsyncFacadeBase : FacadeBase
    {
        // タスク
        private TaskInfo _task = null;

        /// <summary>
        /// タスク
        /// </summary>
        protected TaskInfo Task
        {
            get
            {
                return _task;
            }
        }

        /// <summary>
        /// タスクをセットします。
        /// </summary>
        /// <param name="task">タスク</param>
        internal void SetTask(TaskInfo task)
        {
            _task = task;
        }

        /// <summary>
        /// タスクがキャンセルされていないか確認します。
        /// </summary>
        /// <exception cref="TaskCancelException">タスクキャンセル例外</exception>
        public void CheckCancel()
        {
            if (_task.IsCancel)
            {
                throw new TaskCancelException(_task);
            }
        }
    }
}
