using System;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// タグTを、ファイルパスとタグを指定して削除します。
    /// </summary>
    internal class DeleteFileTagAsyncLogic : AbstractAsyncLogic
    {
        public DeleteFileTagAsyncLogic(AbstractAsyncFacade facade) : base(facade) { }

        public void Execute(string filePath, string tag)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            if (tag == null)
            {
                throw new ArgumentNullException("tag");
            }

            DeletionTagByFileAndTagSql sql = new DeletionTagByFileAndTagSql(filePath, tag);
            DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
