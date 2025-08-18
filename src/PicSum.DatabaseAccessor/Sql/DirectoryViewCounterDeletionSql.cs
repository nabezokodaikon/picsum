using SWF.Core.DatabaseAccessor;

namespace PicSum.DatabaseAccessor.Sql
{

    public sealed class DirectoryViewCounterDeletionSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
DELETE FROM t_directory_view_counter
 WHERE file_id = (SELECT mf.file_id
                    FROM m_file mf
                   WHERE mf.file_path = :file_path
                 )
";

        public DirectoryViewCounterDeletionSql(string directoryPath)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            base.Parameters = [
                SqlUtil.CreateParameter("file_path", directoryPath)
            ];
        }
    }
}
