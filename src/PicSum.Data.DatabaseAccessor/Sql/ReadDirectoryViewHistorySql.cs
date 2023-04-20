using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// フォルダの表示履歴を取得します。
    /// </summary>
    public class ReadDirectoryViewHistorySql : SqlBase<DirectoryViewHistoryDto>
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

        public ReadDirectoryViewHistorySql(int limit)
            : base(SQL_TEXT)
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("limit", limit));
        }
    }
}
