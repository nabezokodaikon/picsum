using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// ファイル表示履歴T作成
    /// </summary>
    public class CreationFileViewHistorySql : SqlBase
    {
        public CreationFileViewHistorySql(string filePath)
            : base()
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", filePath));
        }
    }
}
