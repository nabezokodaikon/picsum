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
        protected override async ValueTask Execute()
        {
            var result = new ListResult<string>(await this.GetTags().False());
            this.Callback(result);
        }

        private async ValueTask<string[]> GetTags()
        {
            var con = await Instance<IFileInfoDao>.Value.Connect().False();
            try
            {
                var logic = new TagsGetLogic(this);
                return await logic.Execute(con).False();
            }
            finally
            {
                await con.DisposeAsync().False();
            }
        }
    }
}
