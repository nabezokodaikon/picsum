using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class BookmarkAddJob
        : AbstractOneWayJob<ValueParameter<string>>
    {
        protected override Task Execute(ValueParameter<string> param)
        {
            if (string.IsNullOrEmpty(param.Value))
            {
                throw new ArgumentNullException(param.Value, nameof(param.Value));
            }

            var registrationDate = DateTime.Now;

            using (var con = Instance<IFileInfoDB>.Value.ConnectWithTransaction())
            {
                var deleteLogic = new BookmarkDeleteLogic(this);
                var addLogic = new BookmarkAddLogic(this);

                if (!deleteLogic.Execute(con, param.Value))
                {
                    if (!addLogic.Execute(con, param.Value, registrationDate))
                    {
                        var addFileMasterLogic = new FileMasterAddLogic(this);
                        addFileMasterLogic.Execute(con, param.Value);

                        addLogic.Execute(con, param.Value, registrationDate);
                    }
                }
                else
                {
                    addLogic.Execute(con, param.Value, registrationDate);
                }

                con.Commit();
            }

            return Task.CompletedTask;
        }
    }
}
