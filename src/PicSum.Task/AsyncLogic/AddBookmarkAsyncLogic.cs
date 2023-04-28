using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;

namespace PicSum.Task.AsyncLogic
{
    internal sealed class AddBookmarkAsyncLogic
        : AsyncLogicBase
    {
        public AddBookmarkAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public bool Execute(string filePath, DateTime registration_date)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var sql = new CreationBookmarkSql(filePath, registration_date);

            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
