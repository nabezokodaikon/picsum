namespace SWF.Core.Job
{
    public sealed class JobCancelException
        : Exception
    {
        internal JobCancelException(string jobName)
            : base($"{jobName} がキャンセルされました。")
        {

        }
    }
}
