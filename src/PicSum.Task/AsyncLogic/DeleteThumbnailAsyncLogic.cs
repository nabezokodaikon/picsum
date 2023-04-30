using System;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// サムネイルを削除します。
    /// </summary>
    public class DeleteThumbnailAsyncLogic:AbstractAsyncLogic
    {
        public DeleteThumbnailAsyncLogic(AbstractAsyncFacade facade) : base(facade) { }

        public void Execute(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            DeletionThumbnailByFileSql sql = new DeletionThumbnailByFileSql(filePath);
            DatabaseManager<ThumbnailConnection>.Update(sql);
        }
    }
}
