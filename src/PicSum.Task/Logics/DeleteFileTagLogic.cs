using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;

namespace PicSum.Task.Logics
{
    /// <summary>
    /// タグTを、ファイルパスとタグを指定して削除します。
    /// </summary>
    internal sealed class DeleteFileTagLogic : AbstractAsyncLogic
    {
        public DeleteFileTagLogic(IAsyncTask task)
            : base(task)
        {

        }

        public void Execute(string filePath, string tag)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            var sql = new DeletionTagByFileAndTagSql(filePath, tag);
            DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
