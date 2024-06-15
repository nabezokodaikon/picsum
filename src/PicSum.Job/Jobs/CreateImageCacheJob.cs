using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Logics;
using SWF.Common;

namespace PicSum.Job.Jobs
{
    public sealed class CreateImageCacheJob
        : AbstractOneWayJob<ListParameter<string>>
    {
        protected override void Execute(ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            var logic = new ImageFileReadLogic(this);

            foreach (var path in parameter)
            {
                Thread.Sleep(10);

                this.CheckCancel();

                try
                {
                    logic.Create(path);
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
