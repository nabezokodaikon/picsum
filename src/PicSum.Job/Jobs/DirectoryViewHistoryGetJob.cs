using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
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

            foreach (var dto in await this.GetHistories().False())
            {
                this.ThrowIfJobCancellationRequested();

                try
                {
                    var info = await logic.Get(dto.DirectoryPath, false).False();
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

        private async ValueTask<DirectoryViewHistoryDto[]> GetHistories()
        {
            var con = await Instance<IFileInfoDao>.Value.Connect().False();
            try
            {
                var logic = new DirectoryViewHistoryGetLogic(this);
                return await logic.Execute(con).False();
            }
            finally
            {
                await con.DisposeAsync().False();
            }
        }
    }
}
