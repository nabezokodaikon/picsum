using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using SWF.Common;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace PicSum.Task.AsyncLogic
{
    [SupportedOSPlatform("windows")]
    internal sealed class GetBookmarkListAsyncLogic
        : AbstractAsyncLogic
    {
        public GetBookmarkListAsyncLogic(AbstractAsyncTask task)
            : base(task)
        {

        }

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
