namespace SWF.Core.Job
{
    public class JobException
        : Exception
    {
        public JobException(JobID id, Exception ex)
            : base($"{id} で例外が発生しました。", ex)
        {

        }

        internal JobException(string message)
            : base(message)
        {

        }
    }
}
