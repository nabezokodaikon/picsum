using PicSum.DatabaseAccessor.Dto;
using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    /// <summary>
    /// 複数ファイルの情報を取得します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class FileTagReadSql
        : SqlBase<FileTagDto>
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

        public FileTagReadSql(string[] filePathList)
            : base(SQL_TEXT)
        {
            ArgumentNullException.ThrowIfNull(filePathList, nameof(filePathList));

            base.ParameterList.Add(SqlUtil.CreateParameter("file_count", filePathList.Length));
            base.ParameterList.AddRange(SqlUtil.CreateParameter("file_path", filePathList));
        }
    }
}
