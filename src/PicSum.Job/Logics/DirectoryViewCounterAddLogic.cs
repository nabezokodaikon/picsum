using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
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
