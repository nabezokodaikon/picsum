namespace PicSum.Core.Job.AsyncJob
{
    public sealed class ValueResult<T>
        : IJobResult
    {
        public T? Value { get; set; }
    }
}
