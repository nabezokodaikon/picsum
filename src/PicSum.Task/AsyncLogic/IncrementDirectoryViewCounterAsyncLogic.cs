using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;

namespace PicSum.Task.AsyncLogic
{
    internal sealed class IncrementDirectoryViewCounterAsyncLogic
        : AbstractAsyncLogic
    {
        public IncrementDirectoryViewCounterAsyncLogic(IAsyncTask task)
            : base(task)
        {

        }

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
