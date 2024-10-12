using SWF.Core.DatabaseAccessor;
using PicSum.DatabaseAccessor.Dto;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
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
