namespace SWF.Core.Job
{
    public struct ValueResult<T>
        : IJobResult
    {
        public T? Value { get; set; }
    }
}
