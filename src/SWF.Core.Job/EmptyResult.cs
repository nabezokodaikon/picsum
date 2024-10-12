namespace SWF.Core.Job
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
