namespace SWF.Core.Job
{
    public sealed class ListParameter<T>
        : List<T>, IJobParameter
    {
        public ListParameter()
        {

        }

        public ListParameter(int capacity)
            : base(capacity)
        {

        }

        public ListParameter(IEnumerable<T> collection)
            : base(collection)
        {

        }
    }
}
