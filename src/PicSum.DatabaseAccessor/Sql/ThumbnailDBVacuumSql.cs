using SWF.Core.DatabaseAccessor;

namespace PicSum.DatabaseAccessor.Sql
{

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
