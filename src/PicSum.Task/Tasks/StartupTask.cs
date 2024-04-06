using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Logics;
using PicSum.Task.Entities;
using PicSum.Task.Paramters;
using System;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// スタートアップ非同期タスク
    /// </summary>
    public sealed class StartupTask
        : AbstractAsyncTask<StartupPrameter>
    {
        protected override void Execute(StartupPrameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var logic = new StartupLogic(this);
            logic.Execute(param);

            this.Callback(EmptyResult.Instance);
        }
    }
}
