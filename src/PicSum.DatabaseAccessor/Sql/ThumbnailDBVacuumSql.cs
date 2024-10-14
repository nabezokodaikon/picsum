using PicSum.DatabaseAccessor.Dto;
using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
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