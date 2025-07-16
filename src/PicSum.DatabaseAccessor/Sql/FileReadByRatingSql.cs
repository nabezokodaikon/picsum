using PicSum.DatabaseAccessor.Dto;
using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    /// <summary>
    /// 評価値を指定してファイルを読込みます。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class FileReadByRatingSql
        : SqlBase<FileByRatingDto>
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

        public FileReadByRatingSql(int param)
            : base(SQL_TEXT)
        {
            base.Parameters = [
                SqlUtil.CreateParameter("rating", param)
            ];
        }
    }
}
