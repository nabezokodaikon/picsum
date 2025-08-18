using SWF.Core.DatabaseAccessor;

namespace PicSum.DatabaseAccessor.Sql
{

    public sealed class ThumbnailOffsetUpdateSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
UPDATE m_thumbnail_id
   SET thumbnail_id = :offset;
";

        public ThumbnailOffsetUpdateSql(int offset)
            : base(SQL_TEXT)
        {
            base.Parameters = [
                SqlUtil.CreateParameter(nameof(offset), offset),
            ];
        }
    }
}
