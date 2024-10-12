using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// タグを更新します。
    /// </summary>
    /// <remarks>タグの存在確認として使用します。</remarks>
    [SupportedOSPlatform("windows")]
    internal sealed class FileTagUpdateLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public bool Execute(string filePath, string tag, DateTime registrationDate)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentException.ThrowIfNullOrEmpty(tag, nameof(tag));

            var sql = new TagUpdateSql(filePath, tag, registrationDate);
            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
