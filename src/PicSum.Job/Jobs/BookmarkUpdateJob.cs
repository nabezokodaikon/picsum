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
        protected override ValueTask Execute(ValueParameter<string> param)
        {
            if (string.IsNullOrEmpty(param.Value))
            {
                throw new ArgumentNullException(param.Value, nameof(param.Value));
            }

            var registrationDate = DateTime.Now;

            using (var con = Instance<IFileInfoDB>.Value.ConnectWithTransaction())
            {
                var updateLogic = new BookmarkUpdateLogic(this);

                if (!updateLogic.Execute(con, param.Value, registrationDate))
                {
                    var addFileMasterLogic = new FileMasterAddLogic(this);
                    addFileMasterLogic.Execute(con, param.Value);

                    updateLogic.Execute(con, param.Value, registrationDate);
                }

                con.Commit();
            }

            return ValueTask.CompletedTask;
        }
    }
}
