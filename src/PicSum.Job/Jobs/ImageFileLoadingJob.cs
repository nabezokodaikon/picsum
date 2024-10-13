using SWF.Core.Job;
using System.Diagnostics;

namespace PicSum.Job.Jobs
{
    public sealed class ImageFileLoadingJob
        : AbstractOneWayJob
    {
        protected override void Execute()
        {
            var sw = Stopwatch.StartNew();
            while (true)
            {
                if (sw.ElapsedMilliseconds > 100)
                {
                    return;
                }

                this.CheckCancel();

                Thread.Sleep(1);
            }
        }
    }
}
