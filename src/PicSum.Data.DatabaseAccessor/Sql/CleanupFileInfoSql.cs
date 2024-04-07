using PicSum.Core.Data.DatabaseAccessor;
using System.Data;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    public sealed class CleanupFileInfoSql
        : SqlBase
    {
        const string SQL_TEXT =
@"
DELETE FROM m_file
 WHERE file_id = :file_id;

DELETE FROM t_bookmark
 WHERE file_id = :file_id;

DELETE FROM t_directory_state
 WHERE file_id = :file_id;

DELETE FROM t_directory_view_counter
 WHERE file_id = :file_id;

DELETE FROM t_directory_view_history
 WHERE file_id = :file_id;

DELETE FROM t_rating
 WHERE file_id = :file_id;

DELETE FROM t_tag
 WHERE file_id = :file_id;
";

        public CleanupFileInfoSql(long fileID)
            : base(SQL_TEXT)
        {
            base.ParameterList.AddRange(new IDbDataParameter[] {
                SqlParameterUtil.CreateParameter("file_id", fileID),
            });
        }
    }
}
