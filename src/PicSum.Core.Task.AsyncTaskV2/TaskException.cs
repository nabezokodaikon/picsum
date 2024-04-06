using SWF.Common;

namespace PicSum.Core.Task.AsyncTaskV2
{
    public class TaskException
        : SWFException
    {
        public TaskException(TaskID id, Exception ex)
            : base($"タスク[{id}]で例外が発生しました。", ex)
        {

        }

        public TaskException(string message)
            : base(message)
        {

        }
    }
}
