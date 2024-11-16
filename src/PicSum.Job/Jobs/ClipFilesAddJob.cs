using PicSum.Job.Common;
using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{
    internal sealed class ClipFilesAddJob
        : AbstractOneWayJob<ListParameter<string>>
    {
        protected override void Execute(ListParameter<string> param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            Instance<IClipFiles>.Value.AddFiles([.. param]);
        }
    }
}
