namespace SWF.Core.Job
{
    public sealed class JobCancelException
        : JobException
    {
        internal JobCancelException(JobID id)
            : base($"{id} がキャンセルされました。")
        {

        }
    }
}
