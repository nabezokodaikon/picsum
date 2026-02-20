using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Logics
{
    internal sealed class DirectoryViewHistoryDeleteLogic(IJob job)
        : AbstractLogic(job)
    {
        public async ValueTask Execute(IConnection con, string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var sql = new DirectoryViewHistoryDeletionSql(filePath);
            await con.Update(sql).False();
        }
    }
}
