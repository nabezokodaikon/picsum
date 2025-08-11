using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Entities;
using PicSum.Job.Logics;
using SWF.Core.Base;
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
        protected override async ValueTask Execute()
        {
            var logic = new FileShallowInfoGetLogic(this);
            var result = new ListResult<FileShallowInfoEntity>();

            foreach (var directoryPath in await this.GetHistories().WithConfig())
            {
                this.ThrowIfJobCancellationRequested();

                try
                {
                    var info = await logic.Get(directoryPath, false).WithConfig();
                    if (info != FileShallowInfoEntity.EMPTY)
                    {
                        result.Add(info);
                    }
                }
                catch (FileUtilException ex)
                {
                    this.WriteErrorLog(ex);
                    continue;
                }
            }

            this.Callback(result);
        }

        private async ValueTask<string[]> GetHistories()
        {
            await using (var con = await Instance<IFileInfoDB>.Value.Connect().WithConfig())
            {
                var logic = new DirectoryViewHistoryGetLogic(this);
                return logic.Execute(con);
            }
        }
    }
}
