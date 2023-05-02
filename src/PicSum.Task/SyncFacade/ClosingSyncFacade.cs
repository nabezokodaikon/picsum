using PicSum.Core.Task.SyncTask;
using PicSum.Task.SyncLogic;

namespace PicSum.Task.SyncFacade
{
    /// <summary>
    /// 終了同期ファサード
    /// </summary>
    public sealed class ClosingSyncFacade
        : AbstractSyncFacade
    {
        public void Execute()
        {
            var logic = new ClosingSyncLogic();
            logic.Execute();
        }
    }
}
