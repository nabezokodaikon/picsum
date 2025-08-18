using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// タグの一覧を取得します。
    /// </summary>

    public sealed class TagsGetJob
        : AbstractTwoWayJob<ListResult<string>>
    {
        protected override ValueTask Execute()
        {
            var result = new ListResult<string>(this.GetTags());
            this.Callback(result);
            return ValueTask.CompletedTask;
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
