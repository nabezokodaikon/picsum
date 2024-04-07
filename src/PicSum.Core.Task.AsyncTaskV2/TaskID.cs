namespace PicSum.Core.Task.AsyncTaskV2
{
    public sealed class TaskID
    {
        private static long currentID = 0;

        public static TaskID GetNew()
        {
            return new TaskID(Interlocked.Increment(ref currentID));
        }

        private readonly long value;

        private TaskID(long value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return this.value.ToString();
        }
    }
}
