using SWF.Core.ConsoleAccessor;
using SWF.Core.Job;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class GCCollectRunJob
        : AbstractOneWayJob
    {
        private static readonly long INTERVAL = 1000 * 5;

        protected override void Execute()
        {
            var sw = Stopwatch.StartNew();
            while (true)
            {
                this.CheckCancel();
                Thread.Sleep(100);
                if (sw.ElapsedMilliseconds > INTERVAL)
                {
                    using (TimeMeasuring.Run(false, "GCCollectRunJob.Execute GC.Collect"))
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                    }

                    sw = Stopwatch.StartNew();
                }
            }
        }
    }
}
