using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;
using System;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// タグを指定してファイルを読込みます。
    /// </summary>
    public sealed class ReadFileByTagSql
        : SqlBase<FileByTagDto>
    {
        private const string SQL_TEXT =
@"
SELECT mf.file_path
      ,tt.tag
      ,tt.registration_date
  FROM m_file mf
       INNER JOIN t_tag tt
         ON tt.file_id = mf.file_id
 WHERE tt.tag = :tag
 ORDER BY tt.registration_date DESC
";

        public ReadFileByTagSql(string tag)
            : base(SQL_TEXT)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            base.ParameterList.Add(SqlParameterUtil.CreateParameter("tag", tag));
        }
    }
}