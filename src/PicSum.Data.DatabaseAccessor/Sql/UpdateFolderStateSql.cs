using System;
using PicSum.Core.Data.DatabaseAccessor;
using System.Data;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// フォルダ状態Tを更新します。
    /// </summary>
    public class UpdateFolderStateSql : SqlBase
    {
        public UpdateFolderStateSql(string folderPath, int sortTypeID, bool isAscending, string selectedFilePath)
            : base()
        {
            base.ParameterList.AddRange(new IDbDataParameter[]
                { SqlParameterUtil.CreateParameter("folder_path", folderPath),
                  SqlParameterUtil.CreateParameter("sort_type_id", sortTypeID),
                  SqlParameterUtil.CreateParameter("is_ascending", isAscending),
                  SqlParameterUtil.CreateParameter("selected_file_path", selectedFilePath) });
        }

        public UpdateFolderStateSql(string folderPath, int sortTypeID, bool isAscending)
            : base()
        {
            base.ParameterList.AddRange(new IDbDataParameter[]
                { SqlParameterUtil.CreateParameter("folder_path", folderPath),
                  SqlParameterUtil.CreateParameter("sort_type_id", sortTypeID),
                  SqlParameterUtil.CreateParameter("is_ascending", isAscending),
                  SqlParameterUtil.CreateParameter("selected_file_path", DBNull.Value) });
        }
    }
}
