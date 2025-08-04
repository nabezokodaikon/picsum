using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class FavoriteDirectoriesGetLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public SingleValueDto<string>[] Execute(IDatabaseConnection con)
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
