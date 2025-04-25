using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    [SupportedOSPlatform("windows10.0.17763.0")]
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

            base.ParameterList.Add(SqlUtil.CreateParameter("file_path", directoryPath));
        }
    }
}
