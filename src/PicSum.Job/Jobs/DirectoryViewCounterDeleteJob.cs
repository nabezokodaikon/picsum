using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows")]
    internal sealed class DirectoryViewCounterDeleteJob
        : AbstractOneWayJob<ListParameter<string>>
    {
        protected override void Execute(ListParameter<string> param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            using (var tran = Instance<IFileInfoDB>.Value.BeginTransaction())
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
