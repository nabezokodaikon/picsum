namespace PicSum.Core.Job.AsyncJob
{
    public sealed class ThreadID
    {
        private static long currentID = 0;

        public static ThreadID GetNew()
        {
            return new ThreadID(Interlocked.Increment(ref currentID));
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
