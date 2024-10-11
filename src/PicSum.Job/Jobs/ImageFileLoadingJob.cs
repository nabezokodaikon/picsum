using PicSum.Core.Job.AsyncJob;

namespace PicSum.Job.Jobs
{
    public sealed class ImageFileLoadingJob
        : AbstractTwoWayJob<EmptyResult>
    {
        protected override void Execute()
        {
            Thread.Sleep(50);

            this.Callback(EmptyResult.Value);
        }
    }
}
