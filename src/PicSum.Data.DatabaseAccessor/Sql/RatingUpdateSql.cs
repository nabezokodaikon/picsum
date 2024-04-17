using PicSum.Core.Data.DatabaseAccessor;
using System;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// 評価T更新
    /// </summary>
    public sealed class RatingUpdateSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
UPDATE t_rating
   SET rating = :rating
      ,registration_date = :registration_date
 WHERE file_id = (SELECT mf.file_id
                    FROM m_file mf
                   WHERE mf.file_path = :file_path
                 )
";

        public RatingUpdateSql(string filePath, int rating, DateTime registrationDate)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            base.ParameterList.AddRange(
                [
                    SqlParameterUtil.CreateParameter("file_path", filePath),
                    SqlParameterUtil.CreateParameter("rating", rating),
                    SqlParameterUtil.CreateParameter("registration_date", registrationDate)
                ]);
        }
    }
}
