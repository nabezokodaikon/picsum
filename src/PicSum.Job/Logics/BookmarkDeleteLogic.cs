using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Logics
{

    internal sealed class BookmarkDeleteLogic(IJob job)
        : AbstractLogic(job)
    {
        public async ValueTask<bool> Execute(IConnection con, string filePath)
        {
            ArgumentNullException.ThrowIfNull(con, nameof(con));
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var sql = new BookmarkDeletionSql(filePath);

            return await con.Update(sql).WithConfig();
        }
    }
}
