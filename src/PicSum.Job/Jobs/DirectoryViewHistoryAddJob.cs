using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class DirectoryViewHistoryAddJob
        : AbstractOneWayJob<ValueParameter<string>>
    {
        protected override Task Execute(ValueParameter<string> param)
        {
            if (string.IsNullOrEmpty(param.Value))
            {
                throw new ArgumentNullException(param.Value, nameof(param.Value));
            }

            using (var con = Instance<IFileInfoDB>.Value.ConnectWithTransaction())
            {
                var addDirectoryViewHistory = new DirectoryViewHistoryAddLogic(this);
                if (!addDirectoryViewHistory.Execute(con, param.Value))
                {
                    var addFileMaster = new FileMasterAddLogic(this);
                    addFileMaster.Execute(con, param.Value);

                    var incrementDirectoryViewCounter = new DirectoryViewCounterIncrementLogic(this);
                    addDirectoryViewHistory.Execute(con, param.Value);
                    incrementDirectoryViewCounter.Execute(con, param.Value);
                }
                else
                {
                    var incrementDirectoryViewCounter = new DirectoryViewCounterIncrementLogic(this);
                    incrementDirectoryViewCounter.Execute(con, param.Value);
                }

                con.Commit();
            }

            return Task.CompletedTask;
        }
    }
}
