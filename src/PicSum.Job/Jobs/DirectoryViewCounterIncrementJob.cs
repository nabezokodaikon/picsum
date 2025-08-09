using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal class DirectoryViewCounterIncrementJob
        : AbstractOneWayJob<ValueParameter<string>>
    {
        protected override async ValueTask Execute(ValueParameter<string> param)
        {
            if (string.IsNullOrEmpty(param.Value))
            {
                throw new ArgumentNullException(param.Value, nameof(param.Value));
            }

            await using (var con = await Instance<IFileInfoDB>.Value.ConnectWithTransaction())
            {
                var incrementDirectoryViewCounter = new DirectoryViewCounterIncrementLogic(this);
                if (!incrementDirectoryViewCounter.Execute(con, param.Value))
                {
                    var addFileMaster = new FileMasterAddLogic(this);
                    addFileMaster.Execute(con, param.Value);
                    incrementDirectoryViewCounter.Execute(con, param.Value);
                }

                con.Commit();
            }
        }
    }
}
