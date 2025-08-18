using SWF.Core.DatabaseAccessor;

namespace PicSum.DatabaseAccessor.Sql
{

    public sealed class ThumbnailDBCleanupSql
        : SqlBase
    {
        const string SQL_TEXT =
@"
UPDATE m_thumbnail_id
   SET thumbnail_id = 0;

DELETE FROM t_thumbnail;
";

        public ThumbnailDBCleanupSql()
            : base(SQL_TEXT)
        {

        }
    }
}
