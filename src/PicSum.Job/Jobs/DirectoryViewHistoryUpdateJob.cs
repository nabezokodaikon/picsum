using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{

    internal sealed class DirectoryViewHistoryUpdateJob
        : AbstractOneWayJob<ValueParameter<string>>
    {
        protected override ValueTask Execute(ValueParameter<string> param)
        {
            if (string.IsNullOrEmpty(param.Value))
            {
                throw new ArgumentNullException(param.Value, nameof(param.Value));
            }

            var ticks = DateTime.Now.Ticks;

            using (var con = Instance<IFileInfoDao>.Value.ConnectWithTransaction())
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

            return ValueTask.CompletedTask;
        }
    }
}
