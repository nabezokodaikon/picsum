using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Logics
{

    internal sealed class BookmarkUpdateLogic(IJob job)
        : AbstractLogic(job)
    {
        public async ValueTask<bool> Execute(IConnection con, string filePath, DateTime addDate)
        {
            ArgumentNullException.ThrowIfNull(con, nameof(con));
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var sql = new BookmarkUpdateSql(filePath, addDate);
            return await con.Update(sql).WithConfig();
        }
    }
}
