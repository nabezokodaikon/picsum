using System;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// スタートアップ非同期ファサード
    /// </summary>
    public class StartupAsyncFacade
        : TwoWayFacadeBase<StartupPrameterEntity, DefaultEntity>
    {
        public override void Execute(StartupPrameterEntity param)
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
