using PicSum.DatabaseAccessor.Dto;
using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    /// <summary>
    /// フォルダの表示履歴を取得します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class DirectoryViewHistoryReadSql
        : SqlBase<DirectoryViewHistoryDto>
    {
        const string SQL_TEXT =
@"
SELECT mf.file_path
  FROM m_file mf
       INNER JOIN (SELECT tfvh.file_id AS file_id
                         ,MAX(tfvh.view_date) AS view_date
                     FROM t_directory_view_history tfvh
                 GROUP BY tfvh.file_id
                 ORDER BY tfvh.view_date DESC
                    LIMIT :limit
                  ) tfvh
               ON tfvh.file_id = mf.file_id
";

        public DirectoryViewHistoryReadSql(int param)
            : base(SQL_TEXT)
        {
            base.Parameters = [
                SqlUtil.CreateParameter("limit", param)
            ];
        }
    }
}
