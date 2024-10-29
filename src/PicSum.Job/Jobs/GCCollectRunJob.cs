using SWF.Core.Base;
using SWF.Core.Job;
using System.Diagnostics;

namespace PicSum.Job.Jobs
{
    internal sealed class GCCollectRunJob
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
                    using (TimeMeasuring.Run(true, "GCCollectRunJob.Execute GC.Collect"))
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }

                    sw = Stopwatch.StartNew();
                }
            }
        }
    }
}
