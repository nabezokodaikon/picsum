using System;

namespace PicSum.Core.Task.AsyncTask
{
    /// <summary>
    /// タスク状態変更イベント
    /// </summary>
    public class TaskStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// タスク
        /// </summary>
        public TaskInfo Task { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="task">タスク</param>
        public TaskStateChangedEventArgs(TaskInfo task)
        {
            this.Task = task ?? throw new ArgumentNullException(nameof(task));
        }
    }
}
