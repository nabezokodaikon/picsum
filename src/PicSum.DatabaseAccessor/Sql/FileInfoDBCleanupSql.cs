using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    [SupportedOSPlatform("windows")]
    public sealed class FileInfoDBCleanupSql
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

        public FileInfoDBCleanupSql(long fileID)
            : base(SQL_TEXT)
        {
            base.ParameterList.AddRange([
                SqlParameterUtil.CreateParameter("file_id", fileID),
            ]);
        }
    }
}
