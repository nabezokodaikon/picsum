using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    /// <summary>
    /// フォルダ状態T作成
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class DirectoryStateCreationSql
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
";

        public DirectoryStateCreationSql(string directoryPath, int sortTypeId, bool isAscending, string selectedFilePath)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));
            ArgumentException.ThrowIfNullOrEmpty(selectedFilePath, nameof(selectedFilePath));

            base.ParameterList.AddRange(
                [SqlUtil.CreateParameter("directory_path", directoryPath),
                    SqlUtil.CreateParameter("sort_type_id", sortTypeId),
                    SqlUtil.CreateParameter("is_ascending", isAscending),
                    SqlUtil.CreateParameter("selected_file_path", selectedFilePath)]);
        }

        public DirectoryStateCreationSql(string directoryPath, int sortTypeId, bool isAscending)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            base.ParameterList.AddRange(
                [SqlUtil.CreateParameter("directory_path", directoryPath),
                    SqlUtil.CreateParameter("sort_type_id", sortTypeId),
                    SqlUtil.CreateParameter("is_ascending", isAscending),
                    SqlUtil.CreateParameter("selected_file_path", DBNull.Value)]);
        }
    }
}
