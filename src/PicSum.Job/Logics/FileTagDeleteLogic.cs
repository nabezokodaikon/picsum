using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// タグTを、ファイルパスとタグを指定して削除します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class FileTagDeleteLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public void Execute(IDatabaseConnection con, string filePath, string tag)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentException.ThrowIfNullOrEmpty(tag, nameof(tag));

            var sql = new TagDeletionSql(filePath, tag);
            con.Update(sql);
        }
    }
}
