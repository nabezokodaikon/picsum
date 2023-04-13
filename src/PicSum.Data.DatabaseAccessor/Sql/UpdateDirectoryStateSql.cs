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
        public UpdateDirectoryStateSql(string directoryPath, int sortTypeID, bool isAscending, string selectedFilePath)
            : base()
        {
            base.ParameterList.AddRange(new IDbDataParameter[]
                { SqlParameterUtil.CreateParameter("directory_path", directoryPath),
                  SqlParameterUtil.CreateParameter("sort_type_id", sortTypeID),
                  SqlParameterUtil.CreateParameter("is_ascending", isAscending),
                  SqlParameterUtil.CreateParameter("selected_file_path", selectedFilePath) });
        }

        public UpdateDirectoryStateSql(string directoryPath, int sortTypeID, bool isAscending)
            : base()
        {
            base.ParameterList.AddRange(new IDbDataParameter[]
                { SqlParameterUtil.CreateParameter("directory_path", directoryPath),
                  SqlParameterUtil.CreateParameter("sort_type_id", sortTypeID),
                  SqlParameterUtil.CreateParameter("is_ascending", isAscending),
                  SqlParameterUtil.CreateParameter("selected_file_path", DBNull.Value) });
        }
    }
}
