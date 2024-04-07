using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    public sealed class ReadAllFilesSql
        : SqlBase<MFileDto>
    {
        const string SQL_TEXT =
@"
SELECT file_id
      ,file_path
  FROM m_file
";

        public ReadAllFilesSql()
            : base(SQL_TEXT)
        {

        }
    }
}
