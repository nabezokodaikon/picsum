using System;

namespace PicSum.Core.Task.Base
{
    public class TaskException
        : Exception
    {
        public TaskException(Exception exception)
            : base("タスクで例外が発生しました。", exception)
        {

        }
    }
}
