using PicSum.DatabaseAccessor.Dto;
using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    /// <summary>
    /// マイコンピュータ、ドライブを除く、
    /// 表示回数の多いフォルダを取得します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class FavoriteDirectoriesReadSql
        : SqlBase<FavoriteDirecotryDto>
    {
        const string SQL_TEXT =
@"
SELECT mf.file_path
      ,t.cnt
  FROM m_file mf
       INNER JOIN (SELECT tfvc.file_id
                         ,tfvc.view_count AS cnt
                     FROM t_directory_view_counter tfvc
                 ORDER BY tfvc.view_count DESC
                    LIMIT :limit
                  ) t
          ON t.file_id = mf.file_id
";

        public FavoriteDirectoriesReadSql(int limit)
            : base(SQL_TEXT)
        {
            base.Parameters = [
                SqlUtil.CreateParameter("limit", limit)
            ];
        }
    }
}
