using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Logics
{

    internal sealed class DirectoryViewCounterIncrementLogic(IJob job)
        : AbstractLogic(job)
    {
        public async ValueTask<bool> Execute(IConnection con, string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            var sql = new DirectoryViewCounterIncrementSql(directoryPath);
            return await con.Update(sql).False();
        }
    }
}
