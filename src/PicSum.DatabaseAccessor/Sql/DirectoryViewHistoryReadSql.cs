using SWF.Core.DatabaseAccessor;
using PicSum.DatabaseAccessor.Dto;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    /// <summary>
    /// フォルダの表示履歴を取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class DirectoryViewHistoryReadSql
        : SqlBase<DirectoryViewHistoryDto>
    {
        const string SQL_TEXT =
@"
SELECT mf.file_path
      ,tfvh.view_date
  FROM m_file mf
       INNER JOIN t_directory_view_history tfvh
          ON tfvh.file_id = mf.file_id       
 ORDER BY tfvh.view_date DESC
 LIMIT :limit
";

        public DirectoryViewHistoryReadSql(int param)
            : base(SQL_TEXT)
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("limit", param));
        }
    }
}
