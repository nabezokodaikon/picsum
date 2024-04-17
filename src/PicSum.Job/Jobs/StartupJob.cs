using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Logics;
using PicSum.Job.Paramters;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// スタートアップ非同期ジョブ
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class StartupJob
        : AbstractOneWayJob<StartupPrameter>
    {
        protected override void Execute(StartupPrameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var logic = new StartupLogic(this);
            logic.Execute(param);
        }
    }
}
