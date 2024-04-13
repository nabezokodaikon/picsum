using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    public sealed class ThumbnailIDUpdateSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
UPDATE m_thumbnail_id
   SET thumbnail_id = thumbnail_id + 1;
";

        public ThumbnailIDUpdateSql()
            : base(SQL_TEXT)
        {

        }
    }
}
