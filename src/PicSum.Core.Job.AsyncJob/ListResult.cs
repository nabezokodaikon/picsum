namespace PicSum.Core.Job.AsyncJob
{
    public sealed class ListResult<T>
        : List<T>, IJobResult
    {
        public ListResult()
        {

        }

        public ListResult(IEnumerable<T> collection)
            : base(collection)
        {

        }
    }
}
