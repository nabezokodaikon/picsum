using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    [SupportedOSPlatform("windows")]
    internal sealed class BookmarkAddLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public bool Execute(string filePath, DateTime registrationDate)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var sql = new BookmarkCreationSql(filePath, registrationDate);
            return Dao<IFileInfoDB>.Instance.Update(sql);
        }
    }
}
