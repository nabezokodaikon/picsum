using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class BookmarksGetLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public BookmarkDto[] Execute(IDatabaseConnection con)
        {
            var sql = new BookmarksReadSql();
            var dtoList = con.ReadList<BookmarkDto>(sql);
            return dtoList;
        }
    }
}
