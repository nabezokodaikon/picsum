using SWF.Core.DatabaseAccessor;

namespace PicSum.DatabaseAccessor.Sql
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
                  ) t
          ON t.file_id = mf.file_id
 ORDER BY t.cnt DESC
         ,mf.file_path ASC
";

        public FavoriteDirectoriesReadSql()
            : base(SQL_TEXT)
        {

        }
    }
}
