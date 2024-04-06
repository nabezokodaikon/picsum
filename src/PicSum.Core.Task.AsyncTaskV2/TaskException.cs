namespace PicSum.Core.Task.AsyncTaskV2
{
    public sealed class TaskException
        : Exception
    {
        public TaskException(Exception ex)
            : base("タスクで例外が発生しました。", ex)
        {

        }
    }
}
