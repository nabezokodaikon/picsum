using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Logics
{

    internal sealed class BookmarksGetLogic(IJob job)
        : AbstractLogic(job)
    {
        public BookmarkDto[] Execute(IConnection con)
        {
            using (TimeMeasuring.Run(true, "BookmarksGetLogic.Execute"))
            {
                var sql = new BookmarksReadSql();
                var dtoList = con.ReadList<BookmarkDto>(sql);
                return dtoList;
            }
        }
    }
}
