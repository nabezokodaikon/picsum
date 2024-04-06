namespace PicSum.Core.Task.AsyncTaskV2
{
    public sealed class ListParameter<T>
        : List<T>, ITaskParameter
    {
        public ListParameter()
        {

        }

        public ListParameter(IEnumerable<T> collection)
            : base(collection)
        {

        }
    }
}
