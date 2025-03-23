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
        protected override void Execute(ValueParameter<string> param)
        {
            if (string.IsNullOrEmpty(param.Value))
            {
                throw new ArgumentNullException(param.Value, nameof(param.Value));
            }

            using (var tran = Instance<IFileInfoDB>.Value.BeginTransaction())
            {
                var addDirectoryViewHistory = new DirectoryViewHistoryAddLogic(this);
                if (!addDirectoryViewHistory.Execute(param.Value))
                {
                    var updateFileMaster = new FileMastercUpdateLogic(this);
                    if (!updateFileMaster.Execute(param.Value))
                    {
                        var addFileMaster = new FileMasterAddLogic(this);
                        addFileMaster.Execute(param.Value);
                    }

                    var addDirectoryViewCounter = new DirectoryViewCounterAddLogic(this);
                    addDirectoryViewHistory.Execute(param.Value);
                    addDirectoryViewCounter.Execute(param.Value);
                }
                else
                {
                    var incrementDirectoryViewCounter = new DirectoryViewCounterIncrementLogic(this);
                    if (!incrementDirectoryViewCounter.Execute(param.Value))
                    {
                        var addDirectoryViewCounter = new DirectoryViewCounterAddLogic(this);
                        addDirectoryViewCounter.Execute(param.Value);
                    }
                }

                tran.Commit();
            }
        }
    }
}
