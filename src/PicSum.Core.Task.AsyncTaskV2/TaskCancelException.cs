namespace PicSum.Core.Task.AsyncTaskV2
{
    public sealed class TaskCancelException
        : Exception
    {
        public TaskCancelException(TaskID id)
            : base($"タスク[{id}]がキャンセルされました。")
        {

        }

        public TaskCancelException()
            : base($"タスクがキャンセルされました。")
        {

        }
    }
}
