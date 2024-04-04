using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// サムネイルを削除します。
    /// </summary>
    public class DeleteThumbnailAsyncLogic
        : AbstractAsyncLogic
    {
        public DeleteThumbnailAsyncLogic(AbstractAsyncTask task)
            : base(task)
        {

        }

        public void Execute(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var sql = new DeletionThumbnailByFileSql(filePath);
            DatabaseManager<ThumbnailConnection>.Update(sql);
        }
    }
}
