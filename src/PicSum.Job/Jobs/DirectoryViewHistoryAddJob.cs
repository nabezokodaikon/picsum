using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows")]
    public sealed class DirectoryViewHistoryAddJob
        : AbstractOneWayJob<ValueParameter<string>>
    {
        protected override void Execute(ValueParameter<string> param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            using (var tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
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
