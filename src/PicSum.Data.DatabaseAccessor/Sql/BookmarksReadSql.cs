using SWF.Core.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;
using System.Runtime.Versioning;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    [SupportedOSPlatform("windows")]
    public sealed class BookmarksReadSql
        : SqlBase<BookmarkDto>
    {
        const string SQL_TEXT =
@"
SELECT mf.file_path
      ,tb.registration_date
  FROM t_bookmark tb
       INNER JOIN m_file mf
               ON tb.file_id = mf.file_id
ORDER BY tb.registration_date DESC
";

        public BookmarksReadSql()
            : base(SQL_TEXT)
        {

        }
    }
}
