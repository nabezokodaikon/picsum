using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class DirectoryViewHistoryUpdateJob
        : AbstractOneWayJob<ValueParameter<string>>
    {
        protected async override ValueTask Execute(ValueParameter<string> param)
        {
            if (string.IsNullOrEmpty(param.Value))
            {
                throw new ArgumentNullException(param.Value, nameof(param.Value));
            }

            var ticks = DateTime.Now.Ticks;

            await using (var con = await Instance<IFileInfoDB>.Value.ConnectWithTransaction().WithConfig())
            {
                var updateDirectoryViewHistory = new DirectoryViewHistoryUpdateLogic(this);
                if (!updateDirectoryViewHistory.Execute(con, param.Value, ticks))
                {
                    var addFileMaster = new FileMasterAddLogic(this);
                    addFileMaster.Execute(con, param.Value);
                    updateDirectoryViewHistory.Execute(con, param.Value, ticks);
                }

                con.Commit();
            }
        }
    }
}
