namespace SWF.Core.Job
{
    public sealed class ValueResult<T>
        : IJobResult
    {
        public T? Value { get; set; }
    }
}
