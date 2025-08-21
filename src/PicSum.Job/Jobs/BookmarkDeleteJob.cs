using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{

    internal sealed class BookmarkDeleteJob
        : AbstractOneWayJob<ListParameter<string>>
    {
        protected override async ValueTask Execute(ListParameter<string> param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            var con = await Instance<IFileInfoDB>.Value.ConnectWithTransaction().ConfigureAwait(false);
            await using (con)
            {
                var deleteLogic = new BookmarkDeleteLogic(this);

                foreach (var filePath in param)
                {
                    await deleteLogic.Execute(con, filePath).WithConfig();
                }

                await con.Commit().WithConfig();
            }
        }
    }
}
