using System;
using System.Data;
using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// タグT作成
    /// </summary>
    public class CreationTagSql : SqlBase
    {
        private const string SQL_TEXT =
@"
INSERT INTO t_tag (
     file_id
    ,tag
    ,registration_date
)
SELECT mf.file_id
      ,:tag
      ,:registration_date
  FROM m_file mf
 WHERE mf.file_path = :file_path
";

        public CreationTagSql(string filePath, string tag, DateTime registration_date)
            : base(SQL_TEXT)
        {
            base.ParameterList.AddRange(new IDbDataParameter[] 
                { SqlParameterUtil.CreateParameter("file_path", filePath), 
                  SqlParameterUtil.CreateParameter("tag", tag),
                  SqlParameterUtil.CreateParameter("registration_date", registration_date)
                });
        }
    }
}
