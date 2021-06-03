using System;
using PicSum.Core.Base.Exception;

namespace PicSum.Core.Task.AsyncTask
{
    /// <summary>
    /// タスクキャンセル例外
    /// </summary>
    internal class TaskCancelException : PSApplicationException
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
        public TaskCancelException(TaskInfo task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            _task = task;
        }
    }
}
