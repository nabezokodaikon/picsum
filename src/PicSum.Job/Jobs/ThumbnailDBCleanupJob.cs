using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Logics;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows")]
    public sealed class ThumbnailDBCleanupJob
        : AbstractOneWayJob<ValueParameter<string>>
    {
        protected override void Execute(ValueParameter<string> param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            try
            {
                var thumbnailLogic = new ThumbnailDBCleanupLogic(this);
                thumbnailLogic.Execute(param.Value);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new JobException(this.ID, ex);
            }
            catch (PathTooLongException ex)
            {
                throw new JobException(this.ID, ex);
            }
            catch (IOException ex)
            {
                throw new JobException(this.ID, ex);
            }
            catch (NotSupportedException ex)
            {
                throw new JobException(this.ID, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new JobException(this.ID, ex);
            }
        }
    }
}
