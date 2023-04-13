using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// フォルダ表示履歴T作成
    /// </summary>
    public class CreationDirectoryViewHistorySql : SqlBase
    {
        public CreationDirectoryViewHistorySql(string directoryPath)
            : base()
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("directory_path", directoryPath));
        }
    }
}
