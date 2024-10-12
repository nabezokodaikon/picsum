using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    [SupportedOSPlatform("windows")]
    internal sealed class BookmarkDeleteLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public bool Execute(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var sql = new BookmarkDeletionSql(filePath);

            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
