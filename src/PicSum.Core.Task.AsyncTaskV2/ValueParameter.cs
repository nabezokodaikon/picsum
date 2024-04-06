namespace PicSum.Core.Task.AsyncTaskV2
{
    public sealed class ValueParameter<T>
        : ITaskParameter
    {
        public T? Value { get; set; }
    }
}
