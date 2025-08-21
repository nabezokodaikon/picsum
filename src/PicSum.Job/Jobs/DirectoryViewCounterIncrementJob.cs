using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{

    internal sealed class DirectoryViewCounterIncrementJob
        : AbstractOneWayJob<ValueParameter<string>>
    {
        protected override async ValueTask Execute(ValueParameter<string> param)
        {
            if (string.IsNullOrEmpty(param.Value))
            {
                throw new ArgumentNullException(param.Value, nameof(param.Value));
            }

            var con = await Instance<IFileInfoDB>.Value.ConnectWithTransaction().ConfigureAwait(false);
            await using (con)
            {
                var incrementDirectoryViewCounter = new DirectoryViewCounterIncrementLogic(this);
                if (!await incrementDirectoryViewCounter.Execute(con, param.Value).WithConfig())
                {
                    var addFileMaster = new FileMasterAddLogic(this);
                    await addFileMaster.Execute(con, param.Value).WithConfig();
                    await incrementDirectoryViewCounter.Execute(con, param.Value).WithConfig();
                }

                await con.Commit().WithConfig();
            }
        }
    }
}
