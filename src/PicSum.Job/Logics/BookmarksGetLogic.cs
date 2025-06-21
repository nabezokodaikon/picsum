using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;
using ZLinq;

namespace PicSum.Job.Logics
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class BookmarksGetLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public BookmarkDto[] Execute(IConnection con)
        {
            var sql = new BookmarksReadSql();
            var dtoList = con.ReadList<BookmarkDto>(sql);
            return dtoList
                .AsValueEnumerable()
                .Where(dto => FileUtil.CanAccess(dto.FilePath))
                .ToArray();
        }
    }
}
