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
        protected override Task Execute()
        {
            var result = new ListResult<string>(this.GetTags());
            this.Callback(result);

            return Task.CompletedTask;
        }

        private string[] GetTags()
        {
            using (var con = Instance<IFileInfoDB>.Value.Connect())
            {
                var logic = new TagsGetLogic(this);
                return logic.Execute(con);
            }
        }
    }
}
