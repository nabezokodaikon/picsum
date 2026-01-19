namespace SWF.Core.Job
{
    public sealed class ListResult<T>
        : List<T>, IJobResult
    {
        public ListResult()
        {

        }

        public ListResult(int capacity)
            : base(capacity)
        {

        }

        public ListResult(T[] collection)
            : base(collection)
        {

        }
    }
}
