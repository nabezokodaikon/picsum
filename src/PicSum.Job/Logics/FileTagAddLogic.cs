using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// タグ追加ロジック
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class FileTagAddLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public bool Execute(string filePath, string tag, DateTime registrationDate)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentException.ThrowIfNullOrEmpty(tag, nameof(tag));

            var sql = new TagCreationSql(filePath, tag, registrationDate);
            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
