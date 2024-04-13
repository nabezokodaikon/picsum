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

        public BookmarkCreationSql(string filePath, DateTime registration_date)
            : base(SQL_TEXT)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            base.ParameterList.AddRange(new IDbDataParameter[]
                { SqlParameterUtil.CreateParameter("file_path", filePath),
                  SqlParameterUtil.CreateParameter("registration_date", registration_date)
                });
        }
    }
}
