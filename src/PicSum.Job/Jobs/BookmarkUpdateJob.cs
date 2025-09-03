using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{

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

            await using (var con = await Instance<IFileInfoDao>.Value.ConnectWithTransaction().WithConfig())
            {
                var updateLogic = new BookmarkUpdateLogic(this);

                if (!await updateLogic.Execute(con, param.Value, addDate).WithConfig())
                {
                    var addFileMasterLogic = new FileMasterAddLogic(this);
                    await addFileMasterLogic.Execute(con, param.Value).WithConfig();

                    await updateLogic.Execute(con, param.Value, addDate).WithConfig();
                }

                await con.Commit().WithConfig();
            }
        }
    }
}
