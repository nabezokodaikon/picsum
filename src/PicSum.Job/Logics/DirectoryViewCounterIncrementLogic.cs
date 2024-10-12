using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    [SupportedOSPlatform("windows")]
    internal sealed class DirectoryViewCounterIncrementLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public bool Execute(string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            var sql = new DirectoryViewCounterIncrementSql(directoryPath);
            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
