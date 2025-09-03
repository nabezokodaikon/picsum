using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using SWF.Core.StringAccessor;
using ZLinq;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// タグの一覧を取得します。
    /// </summary>

    internal sealed class TagsGetLogic(IJob job)
        : AbstractLogic(job)
    {
        public async ValueTask<string[]> Execute(IConnection con)
        {
            var sql = new AllTagsReadSql();
            var dtoList = await con.ReadList<TagInfoDto>(sql).WithConfig();

            return [.. dtoList
                .AsValueEnumerable()
                .GroupBy(static file => file.Tag)
                .Where(static file => file.Any(static f => FileUtil.CanAccess(f.FilePath)))
                .Select(static file => file.First().Tag)
                .OrderBy(static tag => tag, NaturalStringComparer.WINDOWS)];
        }
    }
}
