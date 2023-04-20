using System;
using PicSum.Core.Data.DatabaseAccessor;
using System.Data;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// フォルダ状態Tを更新します。
    /// </summary>
    public class UpdateDirectoryStateSql : SqlBase
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

        public UpdateDirectoryStateSql(string directoryPath, int sortTypeID, bool isAscending, string selectedFilePath)
            : base(SQL_TEXT)
        {
            base.ParameterList.AddRange(new IDbDataParameter[]
                { SqlParameterUtil.CreateParameter("directory_path", directoryPath),
                  SqlParameterUtil.CreateParameter("sort_type_id", sortTypeID),
                  SqlParameterUtil.CreateParameter("is_ascending", isAscending),
                  SqlParameterUtil.CreateParameter("selected_file_path", selectedFilePath) });
        }

        public UpdateDirectoryStateSql(string directoryPath, int sortTypeID, bool isAscending)
            : base(SQL_TEXT)
        {
            base.ParameterList.AddRange(new IDbDataParameter[]
                { SqlParameterUtil.CreateParameter("directory_path", directoryPath),
                  SqlParameterUtil.CreateParameter("sort_type_id", sortTypeID),
                  SqlParameterUtil.CreateParameter("is_ascending", isAscending),
                  SqlParameterUtil.CreateParameter("selected_file_path", DBNull.Value) });
        }
    }
}
