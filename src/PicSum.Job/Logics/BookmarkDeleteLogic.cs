using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Logics
{

    internal sealed class BookmarkDeleteLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public async ValueTask<bool> Execute(IDatabaseConnection con, string filePath)
        {
            ArgumentNullException.ThrowIfNull(con, nameof(con));
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var sql = new BookmarkDeletionSql(filePath);

            return await con.Update(sql).WithConfig();
        }
    }
}
