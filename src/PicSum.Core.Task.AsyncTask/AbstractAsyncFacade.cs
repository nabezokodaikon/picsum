using PicSum.Core.Task.Base;
using System;

namespace PicSum.Core.Task.AsyncTask
{
    /// <summary>
    /// 非同期ファサード基底クラス
    /// </summary>
    public abstract class AbstractAsyncFacade 
        : IFacade
    {
        protected TaskInfo Task { get; private set; } = null;

        /// <summary>
        /// タスクをセットします。
        /// </summary>
        /// <param name="task">タスク</param>
        internal void SetTask(TaskInfo task)
        {
            this.Task = task ?? throw new ArgumentNullException(nameof(task));
        }

        /// <summary>
        /// タスクがキャンセルされていないか確認します。
        /// </summary>
        /// <exception cref="TaskCancelException">タスクキャンセル例外</exception>
        public void CheckCancel()
        {
            if (this.Task.IsCancel)
            {
                throw new TaskCancelException(this.Task);
            }
        }
    }
}
