using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// タグの一覧を取得します。
    /// </summary>
    public sealed class GetTagListAsyncFacade
        : TwoWayFacadeBase<ListEntity<string>>
    {
        public override void Execute()
        {
            var logic = new GetTagListAsyncLogic(this);
            var result = new ListEntity<string>(logic.Execute());
            this.OnCallback(result);
        }
    }
}
