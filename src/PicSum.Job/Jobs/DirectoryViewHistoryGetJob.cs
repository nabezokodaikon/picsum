using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Entities;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// フォルダの表示履歴を取得します。
    /// </summary>

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
                    if (!info.IsEmpty)
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
                return await logic.Execute(con).WithConfig();
            }
        }
    }
}
