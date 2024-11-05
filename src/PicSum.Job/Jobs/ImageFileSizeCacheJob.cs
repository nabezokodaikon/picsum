using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{
    public sealed class ImageFileSizeCacheJob
        : AbstractOneWayJob<ListParameter<string>>
    {
        protected override void Execute(ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            foreach (var path in parameter)
            {
                this.CheckCancel();

                try
                {
                    Instance<IImageFileSizeCacher>.Value.Create(path);
                }
                catch (FileUtilException ex)
                {
                    this.WriteErrorLog(new JobException(this.ID, ex));
                }
                catch (ImageUtilException ex)
                {
                    this.WriteErrorLog(new JobException(this.ID, ex));
                }

                Thread.Sleep(1);
            }
        }
    }
}
