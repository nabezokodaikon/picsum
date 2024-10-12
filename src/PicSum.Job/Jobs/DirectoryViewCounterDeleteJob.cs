using SWF.Core.DatabaseAccessor;
using PicSum.Core.Job.AsyncJob;
using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows")]
    public sealed class DirectoryViewCounterDeleteJob
        : AbstractOneWayJob<ListParameter<string>>
    {
        protected override void Execute(ListParameter<string> param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            using (var tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                var logic = new DirectoryViewCounterDeleteLogic(this);

                foreach (var dir in param)
                {
                    logic.Execute(dir);
                }

                tran.Commit();
            }
        }
    }
}
