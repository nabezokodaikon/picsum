using PicSum.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    [SupportedOSPlatform("windows")]
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
