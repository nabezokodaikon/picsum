using PicSum.Job.Logics;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// タグの一覧を取得します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class TagsGetJob
        : AbstractTwoWayJob<ListResult<string>>
    {
        protected override void Execute()
        {
            var logic = new TagsGetLogic(this);
            var result = new ListResult<string>(logic.Execute());
            this.Callback(result);
        }
    }
}
