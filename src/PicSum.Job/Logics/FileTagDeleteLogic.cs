using PicSum.Core.DatabaseAccessor;
using PicSum.Core.Job.AsyncJob;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// タグTを、ファイルパスとタグを指定して削除します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class FileTagDeleteLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public void Execute(string filePath, string tag)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentException.ThrowIfNullOrEmpty(tag, nameof(tag));

            var sql = new TagDeletionSql(filePath, tag);
            DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
