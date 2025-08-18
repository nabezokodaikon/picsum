using SWF.Core.DatabaseAccessor;

namespace PicSum.DatabaseAccessor.Sql
{

    public sealed class BookmarkDeletionSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
DELETE FROM t_bookmark 
 WHERE file_id = (SELECT mf.file_id
                    FROM m_file mf
                   WHERE mf.file_path = :file_path
                 )
";

        public BookmarkDeletionSql(string filePath)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            base.Parameters = [
                SqlUtil.CreateParameter("file_path", filePath)
            ];
        }
    }
}
