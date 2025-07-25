using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    /// <summary>
    /// ファイルM作成
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class FileCreationSql
        : SqlBase
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
 WHERE true
ON CONFLICT(file_id) DO UPDATE SET
     file_path = :file_path
";

        public FileCreationSql(string filePath)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            base.Parameters = [
                SqlUtil.CreateParameter("file_path", filePath)
            ];
        }
    }
}
