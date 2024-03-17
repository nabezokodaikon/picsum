using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using SWF.Common;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// タグの一覧を取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class GetTagListAsyncLogic
        : AbstractAsyncLogic
    {
        public GetTagListAsyncLogic(AbstractAsyncFacade facade)
            : base(facade)
        {

        }

        public IList<string> Execute()
        {
            var sql = new ReadAllTagSql();
            var dtoList = DatabaseManager<FileInfoConnection>.ReadList<TagInfoDto>(sql);

            var tagList = dtoList
                .Where(dto => FileUtil.IsExists(dto.FilePath))
                .GroupBy(file => file.Tag)
                .Select(file => file.First().Tag)
                .OrderBy(tag => tag)
                .ToList();

            return tagList;
        }
    }
}
