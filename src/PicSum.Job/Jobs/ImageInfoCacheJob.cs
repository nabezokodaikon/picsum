using PicSum.Core.Job.AsyncJob;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;

namespace PicSum.Job.Jobs
{
    public sealed class ImageInfoCacheJob
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
                    var size = ImageFileCacheUtil.GetImageSize(path);
                    ImageInfoCacheUtil.SetImageInfo(path, size);
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
