using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class BookmarkDeleteJob
        : AbstractOneWayJob<ListParameter<string>>
    {
        protected override ValueTask Execute(ListParameter<string> param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            using (var con = Instance<IFileInfoDB>.Value.ConnectWithTransaction())
            {
                var deleteLogic = new BookmarkDeleteLogic(this);

                foreach (var filePath in param)
                {
                    deleteLogic.Execute(con, filePath);
                }

                con.Commit();
            }

            return ValueTask.CompletedTask;
        }
    }
}
