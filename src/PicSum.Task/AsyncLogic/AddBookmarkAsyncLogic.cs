using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;

namespace PicSum.Task.AsyncLogic
{
    internal sealed class AddBookmarkAsyncLogic
        : AbstractAsyncLogic
    {
        public AddBookmarkAsyncLogic(AbstractAsyncTask task)
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
