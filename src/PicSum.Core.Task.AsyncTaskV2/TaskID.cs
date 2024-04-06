namespace PicSum.Core.Task.AsyncTaskV2
{
    public sealed class TaskID
    {
        private static int currentID = 0;

        private static int GetNewID()
        {
            return Interlocked.Increment(ref currentID);
        }

        private readonly int value;

        public TaskID()
        {
            this.value = GetNewID();
        }

        public override string ToString()
        {
            return this.value.ToString();
        }
    }
}
