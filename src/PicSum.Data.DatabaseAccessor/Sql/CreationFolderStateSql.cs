using System;
using System.Data;
using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// フォルダ状態T作成
    /// </summary>
    public class CreationFolderStateSql : SqlBase
    {
        public CreationFolderStateSql(string folderPath, int sortTypeId, bool isAscending, string selectedFilePath)
            : base()
        {
            base.ParameterList.AddRange(new IDbDataParameter[]
                { SqlParameterUtil.CreateParameter("folder_path", folderPath),
                  SqlParameterUtil.CreateParameter("sort_type_id", sortTypeId),
                  SqlParameterUtil.CreateParameter("is_ascending", isAscending),
                  SqlParameterUtil.CreateParameter("selected_file_path", selectedFilePath) });
        }

        public CreationFolderStateSql(string folderPath, int sortTypeId, bool isAscending)
            : base()
        {
            base.ParameterList.AddRange(new IDbDataParameter[]
                { SqlParameterUtil.CreateParameter("folder_path", folderPath),
                  SqlParameterUtil.CreateParameter("sort_type_id", sortTypeId),
                  SqlParameterUtil.CreateParameter("is_ascending", isAscending),
                  SqlParameterUtil.CreateParameter("selected_file_path", DBNull.Value) });
        }
    }
}
