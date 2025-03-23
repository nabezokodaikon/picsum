namespace SWF.Core.Job
{
    public readonly struct EmptyParameter
        : IJobParameter
    {
        public static readonly EmptyResult Value = new();
    }
}
