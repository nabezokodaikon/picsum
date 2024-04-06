using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// タグを更新します。
    /// </summary>
    /// <remarks>タグの存在確認として使用します。</remarks>
    internal sealed class UpdateFileTagAsyncLogic
        : AbstractAsyncLogic
    {
        public UpdateFileTagAsyncLogic(IAsyncTask task)
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

            var sql = new UpdateTagSql(filePath, tag, registrationDate);
            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
