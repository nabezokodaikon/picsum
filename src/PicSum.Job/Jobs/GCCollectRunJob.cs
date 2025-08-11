using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class GCCollectRunJob
        : AbstractOneWayJob
    {
        protected override async ValueTask Execute()
        {
            const int INTERVAL = 1000 * 5;

            while (true)
            {
                this.ThrowIfJobCancellationRequested();

                await Task.Delay(INTERVAL, this.CancellationToken).WithConfig();

                using (TimeMeasuring.Run(false, "GCCollectRunJob.Execute GC.Collect"))
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
            }
        }
    }
}
