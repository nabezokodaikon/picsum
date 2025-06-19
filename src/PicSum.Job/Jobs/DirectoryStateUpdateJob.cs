using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// フォルダ状態更新ジョブ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class DirectoryStateUpdateJob
        : AbstractOneWayJob<DirectoryStateParameter>
    {
        protected override Task Execute(DirectoryStateParameter param)
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
                    var addDirectoryStateLogic = new DirectoryStateAddLogic(this);
                    if (!addDirectoryStateLogic.Execute(con, param))
                    {
                        var addFileMasterLogic = new FileMasterAddLogic(this);
                        addFileMasterLogic.Execute(con, param.DirectoryPath);
                        addDirectoryStateLogic.Execute(con, param);
                    }
                }

                con.Commit();
            }

            return Task.CompletedTask;
        }
    }
}
