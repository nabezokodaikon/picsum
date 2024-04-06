using PicSum.Core.Task.AsyncTask;
using PicSum.Task.Logics;
using PicSum.Task.Entities;
using System.Runtime.Versioning;
using PicSum.Core.Task.AsyncTaskV2;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// タグの一覧を取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetTagListTask
        : AbstractAsyncTask<EmptyParameter, ListResult<string>>
    {
        protected override void Execute()
        {
            var logic = new GetTagListLogic(this);
            var result = new ListResult<string>(logic.Execute());
            this.Callback(result);
        }
    }
}
