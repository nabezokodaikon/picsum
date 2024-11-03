using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows")]
    internal sealed class BookmarkDeleteJob
        : AbstractOneWayJob<ListParameter<string>>
    {
        protected override void Execute(ListParameter<string> param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            using (var tran = Dao<FileInfoDB>.Instance.BeginTransaction())
            {
                var deleteLogic = new BookmarkDeleteLogic(this);

                foreach (var filePath in param)
                {
                    deleteLogic.Execute(filePath);
                }

                tran.Commit();
            }
        }
    }
}
