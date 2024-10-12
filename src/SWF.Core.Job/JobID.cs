namespace SWF.Core.Job
{
    public sealed class JobID
    {
        private static long currentID = 0;

        public static JobID GetNew()
        {
            return new JobID(Interlocked.Increment(ref currentID));
        }

        private readonly long value;

        private JobID(long value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return $"JobID: [{this.value}]";
        }
    }
}
