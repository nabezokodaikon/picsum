using PicSum.Core.Job.AsyncJob;
using System.Diagnostics;

namespace PicSum.Job.Jobs
{
    public sealed class GCRunJob
        : AbstractOneWayJob
    {
        private static readonly long INTERVAL = 1000 * 10;

        protected override void Execute()
        {
            var sw = Stopwatch.StartNew();
            while (true)
            {
                this.CheckCancel();
                Thread.Sleep(10);
                if (sw.ElapsedMilliseconds > INTERVAL)
                {
                    GC.Collect();
                    sw = Stopwatch.StartNew();
                }
            }
        }
    }
}
