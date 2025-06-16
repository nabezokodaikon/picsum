using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class DirectoryViewCounterDeleteJob
        : AbstractOneWayJob<ListParameter<string>>
    {
        protected override Task Execute(ListParameter<string> param)
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

            return Task.CompletedTask;
        }
    }
}
