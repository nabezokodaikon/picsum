using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// タグ追加ロジック
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class FileTagAddLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public bool Execute(IDatabaseConnection con, string filePath, string tag, DateTime registrationDate)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentException.ThrowIfNullOrEmpty(tag, nameof(tag));

            var sql = new TagCreationSql(filePath, tag, registrationDate);
            return con.Update(sql);
        }
    }
}
