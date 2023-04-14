using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// タグの一覧を取得します。
    /// </summary>
    public class GetTagListAsyncFacade
        : TwoWayFacadeBase<ListEntity<string>>
    {
        public override void Execute()
        {
            GetTagListAsyncLogic logic = new GetTagListAsyncLogic(this);
            ListEntity<string> result = new ListEntity<string>(logic.Execute());
            OnCallback(result);
        }
    }
}
