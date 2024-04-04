using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using System.Runtime.Versioning;

namespace PicSum.Task.AsyncTask
{
    /// <summary>
    /// タグの一覧を取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetTagListAsyncTask
        : TwoWayTaskBase<ListEntity<string>>
    {
        public override void Execute()
        {
            var logic = new GetTagListAsyncLogic(this);
            var result = new ListEntity<string>(logic.Execute());
            this.OnCallback(result);
        }
    }
}
