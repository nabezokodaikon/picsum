using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using SWF.Common;
using System.Collections.Generic;

namespace PicSum.Task.AsyncLogic
{
    internal sealed class GetBookmarkListAsyncLogic
        : AsyncLogicBase
    {
        public GetBookmarkListAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public IList<BookmarkDto> Execute()
        {
            var sql = new ReadBookmarkSql();
            var dtoList = DatabaseManager<FileInfoConnection>.ReadList<BookmarkDto>(sql);

            var list = new List<BookmarkDto>();
            foreach (var dto in dtoList)
            {
                if (FileUtil.CanAccess(dto.FilePath))
                {
                    list.Add(dto);
                }
            }

            return list;
        }
    }
}
