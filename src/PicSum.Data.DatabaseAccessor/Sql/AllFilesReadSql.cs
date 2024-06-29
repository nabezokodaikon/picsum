using PicSum.Core.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;
using System.Runtime.Versioning;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    [SupportedOSPlatform("windows")]
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
