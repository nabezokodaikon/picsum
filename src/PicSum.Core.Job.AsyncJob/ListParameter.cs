namespace PicSum.Core.Job.AsyncJob
{
    public sealed class ListParameter<T>
        : List<T>, IJobParameter
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
