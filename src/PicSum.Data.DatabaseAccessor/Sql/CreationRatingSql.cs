using PicSum.Core.Data.DatabaseAccessor;
using System;
using System.Data;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// 評価T作成
    /// </summary>
    public class CreationRatingSql : SqlBase
    {
        const string SQL_TEXT =
@"
INSERT INTO t_rating (
     file_id
    ,rating
    ,registration_date
)
SELECT mf.file_id
      ,:rating
      ,:registration_date
  FROM m_file mf
 WHERE mf.file_path = :file_path
";

        public CreationRatingSql(string filePath, int rating, DateTime registration_date)
            : base(SQL_TEXT)
        {
            base.ParameterList.AddRange(new IDbDataParameter[] {
                SqlParameterUtil.CreateParameter("file_path", filePath),
                SqlParameterUtil.CreateParameter("rating", rating),
                SqlParameterUtil.CreateParameter("registration_date", registration_date)
            });
        }
    }
}
