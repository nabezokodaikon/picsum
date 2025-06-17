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
        protected override async Task Execute()
        {
            const long INTERVAL = 1000 * 5;

            var sw = Stopwatch.StartNew();
            while (true)
            {
                this.CheckCancel();

                await Task.Delay(100);

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
