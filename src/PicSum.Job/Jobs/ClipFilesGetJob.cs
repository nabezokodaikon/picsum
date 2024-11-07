using PicSum.Job.Common;
using PicSum.Job.Entities;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ClipFilesGetJob
        : AbstractTwoWayJob<ListResult<FileShallowInfoEntity>>
    {
        protected override void Execute()
        {
            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ListResult<FileShallowInfoEntity>();
            foreach (var file in Instance<IClipFiles>.Value.GetFiles())
            {
                this.CheckCancel();

                try
                {
                    var info = getInfoLogic.Execute(file.FilePath, file.Timestamp, true);
                    if (info != null)
                    {
                        infoList.Add(info);
                    }
                }
                catch (FileUtilException ex)
                {
                    this.WriteErrorLog(new JobException(this.ID, ex));
                    continue;
                }
            }

            this.Callback(infoList);
        }
    }
}
