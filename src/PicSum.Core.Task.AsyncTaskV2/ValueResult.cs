namespace PicSum.Core.Task.AsyncTaskV2
{
    public sealed class ValueResult<T>
        : ITaskResult
    {
        public T? Value { get; set; }
    }
}
