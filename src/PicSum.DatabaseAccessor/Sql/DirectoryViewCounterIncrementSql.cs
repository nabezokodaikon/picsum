using SWF.Core.DatabaseAccessor;

namespace PicSum.DatabaseAccessor.Sql
{

    public sealed class DirectoryViewCounterIncrementSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
INSERT INTO t_directory_view_counter (
       file_id
      ,view_count
)
SELECT mf.file_id
      ,1
  FROM m_file mf
 WHERE mf.file_path = :file_path
ON CONFLICT(file_id) DO UPDATE SET
    view_count = (SELECT tfvc.view_count + 1
                    FROM t_directory_view_counter tfvc
                   WHERE tfvc.file_id = (SELECT mf.file_id
                                           FROM m_file mf
                                          WHERE mf.file_path = :file_path
										)
                 )
";

        public DirectoryViewCounterIncrementSql(string directoryPath)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            base.Parameters = [
                SqlUtil.CreateParameter("file_path", directoryPath)
            ];
        }
    }
}
