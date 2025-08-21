using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// フォルダ状態更新ジョブ
    /// </summary>

    internal sealed class DirectoryStateUpdateJob
        : AbstractOneWayJob<DirectoryStateParameter>
    {
        protected override async ValueTask Execute(DirectoryStateParameter param)
        {
            if (string.IsNullOrEmpty(param.DirectoryPath))
            {
                throw new ArgumentException("ディレクトリパスがNULLです。", nameof(param));
            }

            await using (var con = await Instance<IFileInfoDB>.Value.ConnectWithTransaction().WithConfig())
            {
                var updateDirectoryState = new DirectoryStateUpdateLogic(this);
                if (!await updateDirectoryState.Execute(con, param).WithConfig())
                {
                    var addFileMasterLogic = new FileMasterAddLogic(this);
                    await addFileMasterLogic.Execute(con, param.DirectoryPath).WithConfig();
                    await updateDirectoryState.Execute(con, param).WithConfig();
                }

                await con.Commit().WithConfig();
            }
        }
    }
}
