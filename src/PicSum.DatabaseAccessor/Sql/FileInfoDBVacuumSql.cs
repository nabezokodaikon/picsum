using SWF.Core.DatabaseAccessor;

namespace PicSum.DatabaseAccessor.Sql
{

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
