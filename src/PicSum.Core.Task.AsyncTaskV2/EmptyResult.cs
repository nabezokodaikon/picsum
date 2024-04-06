namespace PicSum.Core.Task.AsyncTaskV2
{
    public sealed class EmptyResult
        : ITaskResult
    {
        public static readonly EmptyResult Instance = new();

        private EmptyResult()
        {

        }
    }
}
