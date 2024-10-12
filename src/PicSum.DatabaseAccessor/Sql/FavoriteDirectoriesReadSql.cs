using SWF.Core.DatabaseAccessor;
using PicSum.DatabaseAccessor.Dto;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    /// <summary>
    /// マイコンピュータ、ドライブを除く、
    /// 表示回数の多いフォルダを取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class FavoriteDirectoriesReadSql
        : SqlBase<SingleValueDto<string>>
    {
        const string SQL_TEXT =
@"
SELECT mf.file_path
  FROM m_file mf
       INNER JOIN (SELECT tfvc.file_id
                         ,tfvc.view_count AS cnt
                     FROM t_directory_view_counter tfvc
                 ORDER BY tfvc.view_count DESC
                    LIMIT 200
                  ) t
          ON t.file_id = mf.file_id
";

        public FavoriteDirectoriesReadSql()
            : base(SQL_TEXT)
        {

        }
    }
}
