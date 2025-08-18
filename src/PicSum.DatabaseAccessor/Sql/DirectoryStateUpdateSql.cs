using SWF.Core.DatabaseAccessor;

namespace PicSum.DatabaseAccessor.Sql
{
    /// <summary>
    /// フォルダ状態Tを更新します。
    /// </summary>

    public sealed class DirectoryStateUpdateSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
INSERT INTO t_directory_state (
     file_id
    ,sort_type_id
    ,is_ascending
    ,selected_file_path
)
SELECT mf.file_id
      ,:sort_type_id
      ,:is_ascending
      ,:selected_file_path
  FROM m_file mf
 WHERE mf.file_path = :directory_path
ON CONFLICT(file_id) DO UPDATE SET
    sort_type_id = :sort_type_id
   ,is_ascending = :is_ascending
   ,selected_file_path = :selected_file_path
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
