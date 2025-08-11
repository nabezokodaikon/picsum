using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using SWF.Core.Base;
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
        protected async override ValueTask Execute()
        {
            var result = new ListResult<string>(await this.GetTags().WithConfig());
            this.Callback(result);
        }

        private async ValueTask<string[]> GetTags()
        {
            await using (var con = await Instance<IFileInfoDB>.Value.Connect().WithConfig())
            {
                var logic = new TagsGetLogic(this);
                return logic.Execute(con);
            }
        }
    }
}
