using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// 評価値を指定してファイルを読込みます。
    /// </summary>
    public class ReadFileByRatingSql : SqlBase<FileByRatingDto>
    {
        const string SQL_TEXT =
@"
SELECT mf.file_path
      ,tr.registration_date
  FROM m_file mf
       INNER JOIN t_rating tr
          ON tr.file_id = mf.file_id
 WHERE tr.rating = :rating
 ORDER BY tr.registration_date DESC
";

        public ReadFileByRatingSql(int rating)
            : base(SQL_TEXT)
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("rating", rating));
        }
    }
}