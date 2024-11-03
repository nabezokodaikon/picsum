using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows")]
    internal sealed class BookmarkAddJob
        : AbstractOneWayJob<ValueParameter<string>>
    {
        protected override void Execute(ValueParameter<string> param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            var registrationDate = DateTime.Now;

            using (var tran = Dao<FileInfoDB>.Instance.BeginTransaction())
            {
                var deleteLogic = new BookmarkDeleteLogic(this);
                var addLogic = new BookmarkAddLogic(this);

                if (!deleteLogic.Execute(param.Value))
                {
                    if (!addLogic.Execute(param.Value, registrationDate))
                    {
                        var updateFileMaster = new FileMastercUpdateLogic(this);
                        if (!updateFileMaster.Execute(param.Value))
                        {
                            var addFileMasterLogic = new FileMasterAddLogic(this);
                            addFileMasterLogic.Execute(param.Value);
                        }

                        addLogic.Execute(param.Value, registrationDate);
                    }
                }
                else
                {
                    addLogic.Execute(param.Value, registrationDate);
                }

                tran.Commit();
            }
        }
    }
}
