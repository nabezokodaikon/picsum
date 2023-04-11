using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;

namespace PicSum.Task.AsyncLogic
{
    internal sealed class IncrementFolderViewCounterAsyncLogic
        : AsyncLogicBase
    {
        public IncrementFolderViewCounterAsyncLogic(AsyncFacadeBase facade) 
            : base(facade) { }

        public bool Execute(string folderPath)
        {
            if (folderPath == null)
            {
                throw new ArgumentNullException(nameof(folderPath));
            }

            var sql = new IncrementFolderViewCounterSql(folderPath);
            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
