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

            var con = await Instance<IFileInfoDao>.Value.ConnectWithTransaction().False();
            try
            {
                var deleteLogic = new BookmarkDeleteLogic(this);

                foreach (var filePath in param)
                {
                    await deleteLogic.Execute(con, filePath).False();
                }
            }
            finally
            {
                await con.DisposeAsync().False();
            }
        }
    }
}
