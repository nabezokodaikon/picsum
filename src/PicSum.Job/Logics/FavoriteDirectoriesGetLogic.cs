using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Logics
{

    internal sealed class FavoriteDirectoriesGetLogic(IJob job)
        : AbstractLogic(job)
    {
        public async ValueTask<SingleValueDto<string>[]> Execute(IConnection con)
        {
            using (Measuring.Time(true, "FavoriteDirectoriesGetLogic.Execute"))
            {
                var sql = new FavoriteDirectoriesReadSql();
                var dtoList = await con.ReadList<SingleValueDto<string>>(sql).False();
                return dtoList;
            }
        }
    }
}
