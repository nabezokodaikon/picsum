using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;
using System.Runtime.Versioning;

namespace PicSum.Tasks.Logics
{
    [SupportedOSPlatform("windows")]
    internal sealed class AddBookmarkLogic
        : AbstractAsyncLogic
    {
        public AddBookmarkLogic(IAsyncTask task)
            : base(task)
        {

        }

        public bool Execute(string filePath, DateTime registrationDate)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var sql = new CreationBookmarkSql(filePath, registrationDate);

            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
