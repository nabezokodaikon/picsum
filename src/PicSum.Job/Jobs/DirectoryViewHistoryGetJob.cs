using PicSum.Job.Entities;
using PicSum.Job.Logics;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// フォルダの表示履歴を取得します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class DirectoryViewHistoryGetJob
        : AbstractTwoWayJob<ListResult<FileShallowInfoEntity>>
    {
        protected override void Execute()
        {
            var logic = new FileShallowInfoGetLogic(this);
            var result = new ListResult<FileShallowInfoEntity>();

            foreach (var directoryPath in (new DirectoryViewHistoryGetLogic(this)).Execute())
            {
                this.CheckCancel();

                try
                {
                    var info = logic.Get(directoryPath, false);
                    if (info != FileShallowInfoEntity.EMPTY)
                    {
                        result.Add(info);
                    }
                }
                catch (FileUtilException ex)
                {
                    this.WriteErrorLog(new JobException(this.ID, ex));
                    continue;
                }
            }

            this.Callback(result);
        }
    }
}
