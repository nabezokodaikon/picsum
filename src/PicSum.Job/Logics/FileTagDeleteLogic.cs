using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// タグTを、ファイルパスとタグを指定して削除します。
    /// </summary>

    internal sealed class FileTagDeleteLogic(IJob job)
        : AbstractLogic(job)
    {
        public async ValueTask Execute(IConnection con, string filePath, string tag)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentException.ThrowIfNullOrEmpty(tag, nameof(tag));

            var sql = new TagDeletionSql(filePath, tag);
            await con.Update(sql).False();
        }
    }
}
