using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using SWF.Core.Job;
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
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            var logic = new StartupLogic(this);
            logic.Execute(param);
        }
    }
}
