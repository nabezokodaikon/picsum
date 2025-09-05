using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{

    public sealed class GCCollectRunJob
        : AbstractOneWayJob
    {
        protected override async ValueTask Execute()
        {
            const int INTERVAL = 1000 * 5;

            while (true)
            {
                this.ThrowIfJobCancellationRequested();

                await Task.Delay(INTERVAL, this.CancellationToken).False();

                using (Measuring.Time(false, "GCCollectRunJob.Execute GC.Collect"))
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
            }
        }
    }
}
