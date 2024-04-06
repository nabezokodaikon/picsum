using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;

namespace PicSum.Task.AsyncLogic
{
    internal sealed class AddBookmarkAsyncLogic
        : AbstractAsyncLogic
    {
        public AddBookmarkAsyncLogic(IAsyncTask task)
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
