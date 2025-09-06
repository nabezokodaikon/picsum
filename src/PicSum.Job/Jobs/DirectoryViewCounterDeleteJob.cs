using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{

    internal sealed class DirectoryViewCounterDeleteJob
        : AbstractOneWayJob<ListParameter<string>>
    {
        protected override async ValueTask Execute(ListParameter<string> param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            var con = await Instance<IFileInfoDao>.Value.ConnectWithTransaction().False();
            try
            {
                var logic = new DirectoryViewCounterDeleteLogic(this);

                foreach (var dir in param)
                {
                    await logic.Execute(con, dir).False();
                }

                await con.Commit().False();
            }
            finally
            {
                await con.DisposeAsync().False();
            }
        }
    }
}
