using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    /// <summary>
    /// フォルダ表示履歴T作成
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class DirectoryViewHistoryUpdateSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
INSERT INTO t_directory_view_history (
     file_id
    ,view_date_ticks
)
SELECT mf.file_id
      ,:ticks
  FROM m_file mf
 WHERE mf.file_path = :directory_path
ON CONFLICT(file_id) DO UPDATE SET
    view_date_ticks = :ticks
";

        public DirectoryViewHistoryUpdateSql(string directoryPath, long ticks)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            base.Parameters = [
                SqlUtil.CreateParameter("directory_path", directoryPath),
                SqlUtil.CreateParameter("ticks", ticks)
            ];
        }
    }
}
