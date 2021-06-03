using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// フォルダ表示履歴T作成
    /// </summary>
    public class CreationFolderViewHistorySql : SqlBase
    {
        public CreationFolderViewHistorySql(string folderPath)
            : base()
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("folder_path", folderPath));
        }
    }
}
