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
        public void Execute(IConnection con, string filePath, string tag)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentException.ThrowIfNullOrEmpty(tag, nameof(tag));

            var sql = new TagDeletionSql(filePath, tag);
            con.Update(sql);
        }
    }
}
