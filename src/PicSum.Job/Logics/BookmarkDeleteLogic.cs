using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class BookmarkDeleteLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public bool Execute(IConnection con, string filePath)
        {
            ArgumentNullException.ThrowIfNull(con, nameof(con));
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var sql = new BookmarkDeletionSql(filePath);

            return con.Update(sql);
        }
    }
}
