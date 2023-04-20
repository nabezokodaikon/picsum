using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// ファイルM作成
    /// </summary>
    public class CreationFileSql : SqlBase
    {
        private const string SQL_TEXT =
@"
INSERT INTO m_file (
     file_id
    ,file_path
)
SELECT mfi.file_id
      ,:file_path
  FROM m_file_id mfi
";

        public CreationFileSql(string filePath)
            : base(SQL_TEXT)
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", filePath));
        }
    }
}
