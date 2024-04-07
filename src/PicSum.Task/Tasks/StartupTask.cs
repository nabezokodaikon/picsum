using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Logics;
using PicSum.Task.Paramters;
using System;
using System.Runtime.Versioning;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// スタートアップ非同期タスク
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class StartupTask
        : AbstractOneWayTask<StartupPrameter>
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
