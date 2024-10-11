namespace PicSum.Core.Job.AsyncJob
{
    public sealed class EmptyResult
        : IJobResult
    {
        public static readonly EmptyResult Value = new();

        private EmptyResult()
        {

        }
    }
}
