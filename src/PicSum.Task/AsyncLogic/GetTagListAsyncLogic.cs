using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using SWF.Common;
using System.Collections.Generic;
using System.Linq;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// タグの一覧を取得します。
    /// </summary>
    internal class GetTagListAsyncLogic
        : AsyncLogicBase
    {
        public GetTagListAsyncLogic(AsyncFacadeBase facade)
            : base(facade) { }

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
