using PicSum.Core.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;
using System.Runtime.Versioning;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    [SupportedOSPlatform("windows")]
    public sealed class FileInfoDBVacuumSql
        : SqlBase<EmptyDto>
    {
        const string SQL_TEXT =
@"
vacuum;
reindex;
";

        public FileInfoDBVacuumSql()
            : base(SQL_TEXT)
        {

        }
    }
}
