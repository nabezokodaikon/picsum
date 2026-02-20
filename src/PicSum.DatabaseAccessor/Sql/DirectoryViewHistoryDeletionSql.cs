using SWF.Core.DatabaseAccessor;

namespace PicSum.DatabaseAccessor.Sql
{
    /// <summary>
    /// タグTを、ファイルパスとタグを指定して削除します。
    /// </summary>

    public sealed class DirectoryViewHistoryDeletionSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
DELETE FROM t_directory_view_history
 WHERE file_id = (SELECT mf.file_id
                    FROM m_file mf
                   WHERE mf.file_path = :file_path)
";

        public DirectoryViewHistoryDeletionSql(string filePath)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            base.Parameters = [
                SqlUtil.CreateParameter("file_path", filePath)
            ];
        }
    }
}
