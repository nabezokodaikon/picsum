using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class BookmarkUpdateLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public bool Execute(IDatabaseConnection con, string filePath, DateTime registrationDate)
        {
            ArgumentNullException.ThrowIfNull(con, nameof(con));
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var sql = new BookmarkUpdateSql(filePath, registrationDate);
            return con.Update(sql);
        }
    }
}
