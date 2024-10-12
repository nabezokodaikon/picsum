using PicSum.Core.DatabaseAccessor;
using PicSum.Core.Job.AsyncJob;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows")]
    public sealed class BookmarkDeleteJob
        : AbstractOneWayJob<ListParameter<string>>
    {
        protected override void Execute(ListParameter<string> param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            using (var tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
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
