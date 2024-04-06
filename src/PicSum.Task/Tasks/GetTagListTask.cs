using PicSum.Core.Task.AsyncTask;
using PicSum.Task.Logics;
using PicSum.Task.Entities;
using System.Runtime.Versioning;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// タグの一覧を取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetTagListTask
        : TwoWayTaskBase<ListEntity<string>>
    {
        public override void Execute()
        {
            var logic = new GetTagListLogic(this);
            var result = new ListEntity<string>(logic.Execute());
            this.OnCallback(result);
        }
    }
}
