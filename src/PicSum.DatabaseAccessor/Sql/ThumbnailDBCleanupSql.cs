using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    [SupportedOSPlatform("windows10.0.17763.0")]
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
