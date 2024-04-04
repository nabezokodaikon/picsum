using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// タグ追加ロジック
    /// </summary>
    internal sealed class AddFileTagAsyncLogic
        : AbstractAsyncLogic
    {
        public AddFileTagAsyncLogic(AbstractAsyncTask task)
            : base(task)
        {

        }

        public bool Execute(string filePath, string tag, DateTime registrationDate)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            var sql = new CreationTagSql(filePath, tag, registrationDate);
            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
