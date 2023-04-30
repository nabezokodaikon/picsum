using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;

namespace PicSum.Task.AsyncLogic
{
    internal sealed class IncrementDirectoryViewCounterAsyncLogic
        : AbstractAsyncLogic
    {
        public IncrementDirectoryViewCounterAsyncLogic(AbstractAsyncFacade facade) 
            : base(facade) { }

        public bool Execute(string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            var sql = new IncrementDirectoryViewCounterSql(directoryPath);
            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
