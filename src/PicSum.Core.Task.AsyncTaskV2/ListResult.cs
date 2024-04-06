namespace PicSum.Core.Task.AsyncTaskV2
{
    public sealed class ListResult<T>
        : List<T>, ITaskResult
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
