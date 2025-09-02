using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Logics
{

    internal sealed class FavoriteDirectoriesGetLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public SingleValueDto<string>[] Execute(IConnection con)
        {
            using (TimeMeasuring.Run(true, "FavoriteDirectoriesGetLogic.Execute"))
            {
                var sql = new FavoriteDirectoriesReadSql();
                var dtoList = con.ReadList<SingleValueDto<string>>(sql);
                return dtoList;
            }
        }
    }
}
