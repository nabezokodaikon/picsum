using System;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// ファイルの評価値を削除します。
    /// </summary>
    internal class DeleteFileRatingAsyncLogic : AsyncLogicBase
    {
        public DeleteFileRatingAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public void Execute(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            DeletionRatingByFileSql sql = new DeletionRatingByFileSql(filePath);
            DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
