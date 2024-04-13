using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Logics;
using System.Runtime.Versioning;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// タグの一覧を取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class TagsGetTask
        : AbstractTwoWayTask<ListResult<string>>
    {
        protected override void Execute()
        {
            var logic = new TagsGetLogic(this);
            var result = new ListResult<string>(logic.Execute());
            this.Callback(result);
        }
    }
}
