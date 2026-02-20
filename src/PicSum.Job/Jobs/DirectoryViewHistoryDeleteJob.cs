using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// ファイルのタグを削除します。
    /// </summary>

    internal sealed class DirectoryViewHistoryDeleteJob
        : AbstractOneWayJob<ListParameter<string>>
    {
        protected override async ValueTask Execute(ListParameter<string> param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            var con = await Instance<IFileInfoDao>.Value.ConnectWithTransaction().False();
            try
            {
                var logic = new DirectoryViewHistoryDeleteLogic(this);

                foreach (var dir in param)
                {
                    await logic.Execute(con, dir).False();
                }
            }
            finally
            {
                await con.DisposeAsync().False();
            }
        }
    }
}
