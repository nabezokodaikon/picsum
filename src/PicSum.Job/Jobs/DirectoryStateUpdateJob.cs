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
        protected override ValueTask Execute(DirectoryStateParameter param)
        {
            if (string.IsNullOrEmpty(param.DirectoryPath))
            {
                throw new ArgumentException("ディレクトリパスがNULLです。", nameof(param));
            }

            using (var con = Instance<IFileInfoDB>.Value.ConnectWithTransaction())
            {
                var updateDirectoryState = new DirectoryStateUpdateLogic(this);
                if (!updateDirectoryState.Execute(con, param))
                {
                    var addFileMasterLogic = new FileMasterAddLogic(this);
                    addFileMasterLogic.Execute(con, param.DirectoryPath);
                    updateDirectoryState.Execute(con, param);
                }

                con.Commit();
            }

            return ValueTask.CompletedTask;
        }
    }
}
