using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{

    internal sealed class DirectoryViewHistoryUpdateJob
        : AbstractOneWayJob<ValueParameter<string>>
    {
        protected override async ValueTask Execute(ValueParameter<string> param)
        {
            if (string.IsNullOrEmpty(param.Value))
            {
                throw new ArgumentNullException(param.Value, nameof(param.Value));
            }

            var ticks = DateTime.Now.Ticks;

            await using (var con = await Instance<IFileInfoDB>.Value.ConnectWithTransaction().WithConfig())
            {
                var updateDirectoryViewHistory = new DirectoryViewHistoryUpdateLogic(this);
                if (!await updateDirectoryViewHistory.Execute(con, param.Value, ticks).WithConfig())
                {
                    var addFileMaster = new FileMasterAddLogic(this);
                    await addFileMaster.Execute(con, param.Value).WithConfig();
                    await updateDirectoryViewHistory.Execute(con, param.Value, ticks).WithConfig();
                }

                await con.Commit().WithConfig();
            }
        }
    }
}
