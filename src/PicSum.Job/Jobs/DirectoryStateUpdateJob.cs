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

            var con = await Instance<IFileInfoDao>.Value.ConnectWithTransaction().False();
            try
            {
                var updateDirectoryState = new DirectoryStateUpdateLogic(this);
                if (!await updateDirectoryState.Execute(con, param).False())
                {
                    var addFileMasterLogic = new FileMasterAddLogic(this);
                    await addFileMasterLogic.Execute(con, param.DirectoryPath).False();
                    await updateDirectoryState.Execute(con, param).False();
                }
            }
            finally
            {
                await con.DisposeAsync().False();
            }
        }
    }
}
