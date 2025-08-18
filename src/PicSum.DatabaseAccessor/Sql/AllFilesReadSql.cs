using PicSum.DatabaseAccessor.Dto;
using SWF.Core.DatabaseAccessor;

namespace PicSum.DatabaseAccessor.Sql
{

    public sealed class AllFilesReadSql
        : SqlBase<MFileDto>
    {
        const string SQL_TEXT =
@"
SELECT file_id
      ,file_path
  FROM m_file
";

        public AllFilesReadSql()
            : base(SQL_TEXT)
        {

        }
    }
}
