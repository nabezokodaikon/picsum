using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{

    internal sealed class BookmarkUpdateJob
        : AbstractOneWayJob<ValueParameter<string>>
    {
        protected override ValueTask Execute(ValueParameter<string> param)
        {
            if (string.IsNullOrEmpty(param.Value))
            {
                throw new ArgumentNullException(param.Value, nameof(param.Value));
            }

            var addDate = DateTime.Now;

            using (var con = Instance<IFileInfoDao>.Value.ConnectWithTransaction())
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

            return ValueTask.CompletedTask;
        }
    }
}
