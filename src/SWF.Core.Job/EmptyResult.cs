namespace SWF.Core.Job
{
    public readonly struct EmptyResult
        : IJobResult
    {
        public static readonly EmptyResult Value = new();
    }
}
