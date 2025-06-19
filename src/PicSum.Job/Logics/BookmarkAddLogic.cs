using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class BookmarkAddLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public bool Execute(IDBConnection con, string filePath, DateTime registrationDate)
        {
            ArgumentNullException.ThrowIfNull(con, nameof(con));
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var sql = new BookmarkCreationSql(filePath, registrationDate);
            return con.Update(sql);
        }
    }
}
