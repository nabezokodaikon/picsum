using PicSum.Core.DatabaseAccessor;
using PicSum.Core.Job.AsyncJob;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    [SupportedOSPlatform("windows")]
    internal sealed class DirectoryViewCounterAddLogic
        : AbstractAsyncLogic
    {
        public DirectoryViewCounterAddLogic(AbstractAsyncJob job)
            : base(job)
        {

        }

        public bool Execute(string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(directoryPath));

            var sql = new DirectoryViewCounterCreationSql(directoryPath);
            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
