using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    public sealed class ReadBookmarkSql
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

        public ReadBookmarkSql()
            : base(SQL_TEXT)
        {

        }
    }
}
