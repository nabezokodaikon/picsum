using SWF.Common;

namespace PicSum.Core.Job.AsyncJob
{
    public class JobException
        : SWFException
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
