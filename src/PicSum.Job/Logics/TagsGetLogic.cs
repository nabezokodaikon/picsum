using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using SWF.Core.StringAccessor;
using System.Runtime.Versioning;
using ZLinq;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// タグの一覧を取得します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class TagsGetLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public string[] Execute(IConnection con)
        {
            var sql = new AllTagsReadSql();
            var dtoList = con.ReadList<TagInfoDto>(sql);

            return [.. dtoList
                .AsValueEnumerable()
                .GroupBy(file => file.Tag)
                .Where(file => file.Any(f => FileUtil.CanAccess(f.FilePath)))
                .Select(file => file.First().Tag)
                .OrderBy(tag => tag, NaturalStringComparer.WINDOWS)];
        }
    }
}
