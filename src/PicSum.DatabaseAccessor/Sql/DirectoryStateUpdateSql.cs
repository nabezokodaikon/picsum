using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    /// <summary>
    /// フォルダ状態Tを更新します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class DirectoryStateUpdateSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
UPDATE t_directory_state
   SET sort_type_id       = :sort_type_id
      ,is_ascending       = :is_ascending
      ,selected_file_path = :selected_file_path
 WHERE file_id = (SELECT mf.file_id
                    FROM m_file mf
                   WHERE mf.file_path = :directory_path
                 )
";

        public DirectoryStateUpdateSql(string directoryPath, int sortTypeID, bool isAscending, string selectedFilePath)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));
            ArgumentException.ThrowIfNullOrEmpty(selectedFilePath, nameof(selectedFilePath));

            base.Parameters = [
                SqlUtil.CreateParameter("directory_path", directoryPath),
                SqlUtil.CreateParameter("sort_type_id", sortTypeID),
                SqlUtil.CreateParameter("is_ascending", isAscending),
                SqlUtil.CreateParameter("selected_file_path", selectedFilePath)
            ];
        }

        public DirectoryStateUpdateSql(string directoryPath, int sortTypeID, bool isAscending)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            base.Parameters = [
                SqlUtil.CreateParameter("directory_path", directoryPath),
                SqlUtil.CreateParameter("sort_type_id", sortTypeID),
                SqlUtil.CreateParameter("is_ascending", isAscending),
                SqlUtil.CreateParameter("selected_file_path", DBNull.Value)
            ];
        }
    }
}
