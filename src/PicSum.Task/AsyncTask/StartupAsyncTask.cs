using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using System;

namespace PicSum.Task.AsyncTask
{
    /// <summary>
    /// スタートアップ非同期タスク
    /// </summary>
    public sealed class StartupAsyncTask
        : AbstractAsyncTask<StartupPrameter>
    {
        protected override void Execute(StartupPrameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var logic = new StartupAsyncLogic(this);
            logic.Execute(param);
        }
    }
}
