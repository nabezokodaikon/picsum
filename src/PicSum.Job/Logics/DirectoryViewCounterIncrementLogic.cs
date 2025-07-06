using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class DirectoryViewCounterIncrementLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public bool Execute(IDatabaseConnection con, string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            var sql = new DirectoryViewCounterIncrementSql(directoryPath);
            return con.Update(sql);
        }
    }
}
