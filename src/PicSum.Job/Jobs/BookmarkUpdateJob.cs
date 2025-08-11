using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class BookmarkUpdateJob
        : AbstractOneWayJob<ValueParameter<string>>
    {
        protected override async ValueTask Execute(ValueParameter<string> param)
        {
            if (string.IsNullOrEmpty(param.Value))
            {
                throw new ArgumentNullException(param.Value, nameof(param.Value));
            }

            var addDate = DateTime.Now;

            await using (var con = await Instance<IFileInfoDB>.Value.ConnectWithTransaction().WithConfig())
            {
                var updateLogic = new BookmarkUpdateLogic(this);

                if (!updateLogic.Execute(con, param.Value, addDate))
                {
                    var addFileMasterLogic = new FileMasterAddLogic(this);
                    addFileMasterLogic.Execute(con, param.Value);

                    updateLogic.Execute(con, param.Value, addDate);
                }

                con.Commit();
            }
        }
    }
}
