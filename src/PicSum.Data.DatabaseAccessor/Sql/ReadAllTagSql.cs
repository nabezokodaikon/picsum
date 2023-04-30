using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// 全てのタグを読込みます。
    /// </summary>
    public sealed class ReadAllTagSql
        : SqlBase<TagInfoDto>
    {
        const string SQL_TEXT =
@"
SELECT mf.file_path
      ,tt.tag
  FROM m_file mf
       INNER JOIN t_tag tt
         ON tt.file_id = mf.file_id
";

        public ReadAllTagSql()
            : base(SQL_TEXT)
        {

        }
    }
}
