namespace PicSum.Core.Task.AsyncTaskV2
{
    public sealed class EmptyParameter
        : ITaskParameter
    {
        public static readonly EmptyParameter Instance = new();

        private EmptyParameter()
        {

        }
    }
}
