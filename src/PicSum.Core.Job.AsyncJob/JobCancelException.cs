namespace PicSum.Core.Job.AsyncJob
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
