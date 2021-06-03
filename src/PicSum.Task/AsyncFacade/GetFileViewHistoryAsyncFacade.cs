using System;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// ファイル表示履歴を取得します。
    /// </summary>
    public class GetFileViewHistoryAsyncFacade
        : TwoWayFacadeBase<ListEntity<DateTime>>
    {
        public override void Execute()
        {
            GetFileViewHistoryAsyncLogic logic = new GetFileViewHistoryAsyncLogic(this);
            ListEntity<DateTime> result = new ListEntity<DateTime>(logic.Execute());
            result.Sort((x, y) => -x.CompareTo(y));
            OnCallback(result);
        }
    }
}
