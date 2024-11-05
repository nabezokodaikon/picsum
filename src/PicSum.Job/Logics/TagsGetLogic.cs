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
    [SupportedOSPlatform("windows")]
    internal sealed class TagsGetLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public IList<string> Execute()
        {
            var sql = new AllTagsReadSql();
            var dtoList = Instance<IFileInfoDB>.Value.ReadList<TagInfoDto>(sql);

            return [.. dtoList
                .Where(dto => FileUtil.IsExists(dto.FilePath))
                .GroupBy(file => file.Tag)
                .Select(file => file.First().Tag)
                .OrderBy(tag => tag)];
        }
    }
}
