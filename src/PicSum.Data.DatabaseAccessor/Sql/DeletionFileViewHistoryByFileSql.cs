using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// ファイル指定ファイル表示履歴T削除
    /// </summary>
    public class DeletionFileViewHistoryByFileSql : SqlBase
    {
        public DeletionFileViewHistoryByFileSql(string filePath)
            : base()
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", filePath));
        }
    }
}