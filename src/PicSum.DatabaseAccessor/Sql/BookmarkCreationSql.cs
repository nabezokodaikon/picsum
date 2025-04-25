using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    [SupportedOSPlatform("windows10.0.17763.0")]
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
                [SqlUtil.CreateParameter("file_path", filePath),
                    SqlUtil.CreateParameter("registration_date", registrationDate)
                ]);
        }
    }
}
