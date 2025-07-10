namespace SWF.Core.Job
{
    public sealed class EmptyParameter
        : IJobParameter
    {
        public static readonly EmptyResult VALUE = new();
    }
}
