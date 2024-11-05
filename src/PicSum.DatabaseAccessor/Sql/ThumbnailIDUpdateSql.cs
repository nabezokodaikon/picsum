using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    [SupportedOSPlatform("windows10.0.17763.0")]
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
