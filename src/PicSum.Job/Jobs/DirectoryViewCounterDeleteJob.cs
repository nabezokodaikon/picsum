using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{

    internal sealed class DirectoryViewCounterDeleteJob
        : AbstractOneWayJob<ListParameter<string>>
    {
        protected override ValueTask Execute(ListParameter<string> param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            using (var con = Instance<IFileInfoDB>.Value.ConnectWithTransaction())
            {
                var logic = new DirectoryViewCounterDeleteLogic(this);

                foreach (var dir in param)
                {
                    logic.Execute(con, dir);
                }

                con.Commit();
            }

            return ValueTask.CompletedTask;
        }
    }
}
