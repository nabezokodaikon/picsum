using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Logics
{

    internal sealed class DirectoryViewCounterIncrementLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public async ValueTask<bool> Execute(IConnection con, string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            var sql = new DirectoryViewCounterIncrementSql(directoryPath);
            return await con.Update(sql).WithConfig();
        }
    }
}
