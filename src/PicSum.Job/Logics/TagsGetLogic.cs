using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// タグの一覧を取得します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class TagsGetLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public string[] Execute()
        {
            var sql = new AllTagsReadSql();
            var dtoList = Instance<IFileInfoDB>.Value.ReadList<TagInfoDto>(sql);

            return [.. dtoList
                .GroupBy(file => file.Tag)
                .Where(file => file.Any(f => FileUtil.IsExists(f.FilePath)))
                .Select(file => file.First().Tag)
                .OrderBy(tag => tag)];
        }
    }
}
