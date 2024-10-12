using SWF.Core.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;
using System.Runtime.Versioning;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    [SupportedOSPlatform("windows")]
    public sealed class ThumbnailDBVacuumSql
        : SqlBase<EmptyDto>
    {
        const string SQL_TEXT =
@"
vacuum;
reindex;
";

        public ThumbnailDBVacuumSql()
            : base(SQL_TEXT)
        {

        }
    }
}
