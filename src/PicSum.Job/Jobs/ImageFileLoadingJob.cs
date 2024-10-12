using PicSum.Core.Job.AsyncJob;
using System.Diagnostics;

namespace PicSum.Job.Jobs
{
    public sealed class ImageFileLoadingJob
        : AbstractTwoWayJob<EmptyResult>
    {
        protected override void Execute()
        {
            var sw = Stopwatch.StartNew();

            try
            {
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
            catch (JobCancelException)
            {

            }
            finally
            {
                this.Callback(EmptyResult.Value);
            }
        }
    }
}
