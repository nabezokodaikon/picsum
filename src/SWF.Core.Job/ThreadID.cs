namespace SWF.Core.Job
{
    public sealed class ThreadID
    {
        private static long _currentID = 0;

        public static ThreadID GetNew()
        {
            return new ThreadID(Interlocked.Increment(ref _currentID));
        }

        private readonly long value;

        private ThreadID(long value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return $"ThreadID: [{this.value}]";
        }
    }
}
