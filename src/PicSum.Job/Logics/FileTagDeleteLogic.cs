using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
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
            Dao<FileInfoDB>.Instance.Update(sql);
        }
    }
}
