namespace PicSum.Core.Task.AsyncTaskV2
{
    public sealed class TaskCancelException
        : TaskException
    {
        internal TaskCancelException(TaskID id)
            : base($"タスク[{id}]がキャンセルされました。")
        {

        }
    }
}
