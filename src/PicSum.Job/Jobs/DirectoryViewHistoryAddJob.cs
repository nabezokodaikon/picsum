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
                    var updateFileMaster = new FileMastercUpdateLogic(this);
                    if (!updateFileMaster.Execute(con, param.Value))
                    {
                        var addFileMaster = new FileMasterAddLogic(this);
                        addFileMaster.Execute(con, param.Value);
                    }

                    var addDirectoryViewCounter = new DirectoryViewCounterAddLogic(this);
                    addDirectoryViewHistory.Execute(con, param.Value);
                    addDirectoryViewCounter.Execute(con, param.Value);
                }
                else
                {
                    var incrementDirectoryViewCounter = new DirectoryViewCounterIncrementLogic(this);
                    if (!incrementDirectoryViewCounter.Execute(con, param.Value))
                    {
                        var addDirectoryViewCounter = new DirectoryViewCounterAddLogic(this);
                        addDirectoryViewCounter.Execute(con, param.Value);
                    }
                }

                con.Commit();
            }

            return Task.CompletedTask;
        }
    }
}
