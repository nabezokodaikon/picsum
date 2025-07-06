using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class DirectoryViewCounterAddLogic
        : AbstractAsyncLogic
    {
        public DirectoryViewCounterAddLogic(IAsyncJob job)
            : base(job)
        {

        }

        public bool Execute(IDatabaseConnection con, string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(directoryPath));

            var sql = new DirectoryViewCounterCreationSql(directoryPath);
            return con.Update(sql);
        }
    }
}
