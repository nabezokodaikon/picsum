using System;
using System.Data;
using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// タグT更新
    /// </summary>
    /// <remarks>タグの存在確認として使用します。</remarks>
    public class UpdateTagSql : SqlBase
    {
        private const string SQL_TEXT =
@"
UPDATE t_tag
   SET tag = :tag
      ,registration_date = :registration_date
 WHERE file_id = (SELECT mf.file_id
                    FROM m_file mf
                   WHERE mf.file_path = :file_path
                 )
   AND tag = :tag
";

        public UpdateTagSql(string filePath, string tag, DateTime registrationDate)
            : base(SQL_TEXT)
        {
            base.ParameterList.AddRange(new IDbDataParameter[]
            {
                SqlParameterUtil.CreateParameter("file_path", filePath), 
                SqlParameterUtil.CreateParameter("tag", tag),
                SqlParameterUtil.CreateParameter("registration_date", registrationDate)
            });
        }
    }
}
