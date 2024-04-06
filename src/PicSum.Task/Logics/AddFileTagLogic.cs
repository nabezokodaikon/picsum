using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;

namespace PicSum.Task.Logics
{
    /// <summary>
    /// タグ追加ロジック
    /// </summary>
    internal sealed class AddFileTagLogic
        : AbstractAsyncLogic
    {
        public AddFileTagLogic(IAsyncTask task)
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