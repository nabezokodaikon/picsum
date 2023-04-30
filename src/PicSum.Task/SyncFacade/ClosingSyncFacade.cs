using System;
using PicSum.Core.Task.SyncTask;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using PicSum.Task.SyncLogic;

namespace PicSum.Task.SyncFacade
{
    /// <summary>
    /// 終了同期ファサード
    /// </summary>
    public class ClosingSyncFacade : AbstractSyncFacade
    {
        public void Execute(ClosingParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            ClosingSyncLogic logic = new ClosingSyncLogic();
            logic.Execute(param);
        }
    }
}
