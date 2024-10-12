using SWF.Core.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;
using System.Runtime.Versioning;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// 全てのタグを読込みます。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class AllTagsReadSql
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

        public AllTagsReadSql()
            : base(SQL_TEXT)
        {

        }
    }
}
