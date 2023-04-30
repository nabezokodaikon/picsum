using PicSum.Core.Base.Exception;

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
            : base(string.Format(
                "タスクがキャンセルされました。タスクID: '{0}', タスク型: '{1}'",
                task.TaskId, task.ProcessType))
        {
            this.Task = task;
        }
    }
}
