using PicSum.Core.Data.DatabaseAccessor;
using System;
using System.Data;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    public sealed class BookmarkCreationSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
INSERT INTO t_bookmark (
     file_id
    ,registration_date
)
SELECT mf.file_id
      ,:registration_date
  FROM m_file mf
 WHERE mf.file_path = :file_path
";

        public BookmarkCreationSql(string filePath, DateTime registrationDate)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            base.ParameterList.AddRange(
                [ SqlParameterUtil.CreateParameter("file_path", filePath),
                  SqlParameterUtil.CreateParameter("registration_date", registrationDate)
                ]);
        }
    }
}
