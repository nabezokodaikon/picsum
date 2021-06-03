using System;

namespace PicSum.Core.Task.AsyncTask
{
    /// <summary>
    /// タスク状態変更イベント
    /// </summary>
    public class TaskStateChangedEventArgs : EventArgs
    {
        // タスク
        private readonly TaskInfo _task;

        /// <summary>
        /// タスク
        /// </summary>
        public TaskInfo Task
        {
            get
            {
                return _task;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="task">タスク</param>
        public TaskStateChangedEventArgs(TaskInfo task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            _task = task;
        }
    }
}
