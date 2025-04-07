using PicSum.Job.Common;
using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{
    public sealed class ImageFileCacheJob
        : AbstractOneWayJob<ListParameter<string>>
    {
        protected override void Execute(ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            Instance<IImageFileCacheThreads>.Value.DoCache([.. parameter]);

            while (true)
            {
                try
                {
                    this.CheckCancel();
                }
                catch (JobCancelException)
                {
                    return;
                }

                Thread.Sleep(10);
            }
        }
    }
}
