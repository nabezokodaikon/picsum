using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;

namespace PicSum.Task.AsyncLogic
{
    internal sealed class DeleteBookmarkAsyncLogic
        : AsyncLogicBase
    {
        public DeleteBookmarkAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public bool Execute(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var sql = new DeletionBookmarkSql(filePath);

            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
