using PicSum.Core.Job.AsyncJob;

namespace PicSum.Job.Jobs
{
    public sealed class ImageFileLoadingJob
        : AbstractTwoWayJob<EmptyResult>
    {
        protected override void Execute()
        {
            Thread.Sleep(30);

            this.Callback(EmptyResult.Value);
        }
    }
}
