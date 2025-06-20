using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    /// <summary>
    /// 複数ファイルの情報を取得します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class FileTagReadSql
        : SqlBase<SingleValueDto<string>>
    {
        private const string SQL_TEXT =
@"
SELECT tt.tag
  FROM m_file mf
       INNER JOIN t_tag tt
          ON tt.file_id = mf.file_id
 WHERE {mf.file_path = :file_path}
";

        public FileTagReadSql(string[] filePathList)
            : base(SQL_TEXT)
        {
            ArgumentNullException.ThrowIfNull(filePathList, nameof(filePathList));

            base.ParameterList.AddRange(SqlUtil.CreateParameter("file_path", filePathList));
        }
    }
}
