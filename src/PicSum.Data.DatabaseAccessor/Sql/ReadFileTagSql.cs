using System.Collections.Generic;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// 複数ファイルの情報を取得します。
    /// </summary>
    public class ReadFileTagSql : SqlBase<FileTagDto>
    {
        private const string SQL_TEXT =
@"
SELECT tt.tag
      ,CASE COUNT(1) WHEN :file_count THEN 'TRUE'
                     ELSE 'FALSE'
        END AS is_all
  FROM m_file mf
       INNER JOIN t_tag tt
          ON tt.file_id = mf.file_id
 WHERE {mf.file_path = :file_path}
 GROUP BY tt.tag
";

        public ReadFileTagSql(IList<string> filePathList)
            : base(SQL_TEXT)
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_count", filePathList.Count));
            base.ParameterList.AddRange(SqlParameterUtil.CreateParameter("file_path", filePathList));
        }
    }
}
