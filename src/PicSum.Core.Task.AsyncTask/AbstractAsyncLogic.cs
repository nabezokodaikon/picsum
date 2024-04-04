using PicSum.Core.Task.Base;
using System;

namespace PicSum.Core.Task.AsyncTask
{
    /// <summary>
    /// 非同期ロジック基底クラス
    /// </summary>
    public abstract class AbstractAsyncLogic
        : ILogic
    {
        // タスク
        private readonly AbstractAsyncTask task;

        /// <summary>
        /// タスクがキャンセルされていないか確認します。
        /// </summary>
        /// <exception cref="TaskCancelException">タスクキャンセル例外</exception>
        protected void CheckCancel()
        {
            this.task.CheckCancel();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="task">タスク</param>
        public AbstractAsyncLogic(AbstractAsyncTask task)
        {
            this.task = task ?? throw new ArgumentNullException("task");
        }
    }
}
