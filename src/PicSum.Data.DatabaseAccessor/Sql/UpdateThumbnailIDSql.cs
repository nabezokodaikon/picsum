using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    public sealed class UpdateThumbnailIDSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
UPDATE m_thumbnail_id
   SET thumbnail_id = thumbnail_id + 1;
";

        public UpdateThumbnailIDSql()
            : base(SQL_TEXT)
        {

        }
    }
}
