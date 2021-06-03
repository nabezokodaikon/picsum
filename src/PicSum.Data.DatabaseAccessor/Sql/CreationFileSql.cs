using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// ファイルM作成
    /// </summary>
    public class CreationFileSql : SqlBase
    {
        public CreationFileSql(string filePath)
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", filePath));
        }
    }
}
