namespace PicSum.Core.Task.AsyncTaskV2
{
    public sealed class TaskException
        : Exception
    {
        public TaskException(TaskID id, Exception ex)
            : base($"タスク[{id}]で例外が発生しました。", ex)
        {

        }
    }
}
