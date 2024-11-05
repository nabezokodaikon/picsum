using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    [SupportedOSPlatform("windows")]
    internal sealed class DirectoryViewCounterIncrementLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public bool Execute(string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            var sql = new DirectoryViewCounterIncrementSql(directoryPath);
            return Instance<IFileInfoDB>.Value.Update(sql);
        }
    }
}
