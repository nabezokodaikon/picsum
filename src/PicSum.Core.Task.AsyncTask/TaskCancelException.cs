using PicSum.Core.Base.Exception;
using System;

namespace PicSum.Core.Task.AsyncTask
{
    /// <summary>
    /// タスクキャンセル例外
    /// </summary>
    internal sealed class TaskCancelException
        : PicSumException
    {
        /// <summary>
        /// タスク
        /// </summary>
        public TaskInfo Task { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="task">タスク</param>
        public TaskCancelException(TaskInfo task)
        {
            this.Task = task ?? throw new ArgumentNullException(nameof(task));
        }
    }
}
