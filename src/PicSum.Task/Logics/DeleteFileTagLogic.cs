using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;
using System.Runtime.Versioning;

namespace PicSum.Task.Logics
{
    /// <summary>
    /// タグTを、ファイルパスとタグを指定して削除します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class DeleteFileTagLogic(IAsyncTask task)
        : AbstractAsyncLogic(task)
    {
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
