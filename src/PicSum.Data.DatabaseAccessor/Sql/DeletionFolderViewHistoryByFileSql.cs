using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// ファイル指定フォルダ表示履歴削除
    /// </summary>
    public class DeletionFolderViewHistoryByFileSql : SqlBase
    {
        public DeletionFolderViewHistoryByFileSql(string filePath)
            : base()
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", filePath));
        }
    }
}
