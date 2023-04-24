using System;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// スタートアップ非同期ファサード
    /// </summary>
    public class StartupAsyncFacade
        : TwoWayFacadeBase<StartupPrameter, DefaultEntity>
    {
        public override void Execute(StartupPrameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            StartupAsyncLogic logic = new StartupAsyncLogic(this);
            logic.Execute(param);

            OnCallback(new DefaultEntity());
        }
    }
}
