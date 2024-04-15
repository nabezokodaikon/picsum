using SWF.Common;

namespace PicSum.Core.Task.AsyncTaskV2
{
    public class TaskException
        : SWFException
    {
        public TaskException(TaskID id, Exception ex)
            : base($"{id} で例外が発生しました。", ex)
        {

        }

        internal TaskException(string message)
            : base(message)
        {

        }
    }
}
