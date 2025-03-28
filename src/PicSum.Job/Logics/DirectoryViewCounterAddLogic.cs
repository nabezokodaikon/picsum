using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
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

        public bool Execute(string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(directoryPath));

            var sql = new DirectoryViewCounterCreationSql(directoryPath);
            return Instance<IFileInfoDB>.Value.Update(sql);
        }
    }
}
