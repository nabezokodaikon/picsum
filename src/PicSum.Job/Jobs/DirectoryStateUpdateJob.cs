using PicSum.Core.DatabaseAccessor;
using PicSum.Core.Job.AsyncJob;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// フォルダ状態更新ジョブ
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class DirectoryStateUpdateJob
        : AbstractOneWayJob<DirectoryStateParameter>
    {
        protected override void Execute(DirectoryStateParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            if (param.DirectoryPath == null)
            {
                throw new ArgumentException("ディレクトリパスがNULLです。", nameof(param));
            }

            using (var tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                var updateDirectoryState = new DirectoryStateUpdateLogic(this);
                if (!updateDirectoryState.Execute(param))
                {
                    var addDirectoryStateLogic = new DirectoryStateAddLogic(this);
                    if (!addDirectoryStateLogic.Execute(param))
                    {
                        var addFileMasterLogic = new FileMasterAddLogic(this);
                        addFileMasterLogic.Execute(param.DirectoryPath);
                        addDirectoryStateLogic.Execute(param);
                    }
                }

                tran.Commit();
            }
        }
    }
}
