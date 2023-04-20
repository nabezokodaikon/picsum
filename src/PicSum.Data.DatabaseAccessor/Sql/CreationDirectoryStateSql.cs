using System;
using System.Data;
using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// フォルダ状態T作成
    /// </summary>
    public class CreationDirectoryStateSql : SqlBase
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

        public CreationDirectoryStateSql(string directoryPath, int sortTypeId, bool isAscending, string selectedFilePath)
            : base(SQL_TEXT)
        {
            base.ParameterList.AddRange(new IDbDataParameter[]
                { SqlParameterUtil.CreateParameter("directory_path", directoryPath),
                  SqlParameterUtil.CreateParameter("sort_type_id", sortTypeId),
                  SqlParameterUtil.CreateParameter("is_ascending", isAscending),
                  SqlParameterUtil.CreateParameter("selected_file_path", selectedFilePath) });
        }

        public CreationDirectoryStateSql(string directoryPath, int sortTypeId, bool isAscending)
            : base(SQL_TEXT)
        {
            base.ParameterList.AddRange(new IDbDataParameter[]
                { SqlParameterUtil.CreateParameter("directory_path", directoryPath),
                  SqlParameterUtil.CreateParameter("sort_type_id", sortTypeId),
                  SqlParameterUtil.CreateParameter("is_ascending", isAscending),
                  SqlParameterUtil.CreateParameter("selected_file_path", DBNull.Value) });
        }
    }
}
