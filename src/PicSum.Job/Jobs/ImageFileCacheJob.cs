using PicSum.Core.Job.AsyncJob;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;

namespace PicSum.Job.Jobs
{
    public sealed class ImageFileCacheJob
        : AbstractOneWayJob<ListParameter<string>>
    {
        protected override void Execute(ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            foreach (var path in parameter)
            {
                this.CheckCancel();

                Thread.Sleep(10);

                try
                {
                    ImageFileCacheUtil.Create(path);

                }
                catch (FileUtilException ex)
                {
                    this.WriteErrorLog(new JobException(this.ID, ex));
                }
                catch (ImageUtilException ex)
                {
                    this.WriteErrorLog(new JobException(this.ID, ex));
                }
            }
        }
    }
}
