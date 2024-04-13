using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// マイコンピュータ、ドライブを除く、
    /// 表示回数の多いフォルダを取得します。
    /// </summary>
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
