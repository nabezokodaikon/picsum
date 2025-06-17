namespace SWF.Core.Job
{
    public sealed class TaskID
    {
        private static long _currentID = 0;

        public static TaskID GetNew()
        {
            return new TaskID(Interlocked.Increment(ref _currentID));
        }

        private readonly long value;

        private TaskID(long value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return $"TaskID: [{this.value}]";
        }
    }
}
