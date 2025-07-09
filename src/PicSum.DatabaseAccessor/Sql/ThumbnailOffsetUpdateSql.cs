using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    [SupportedOSPlatform("windows10.0.17763.0")]
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
            base.ParameterList.AddRange([
                SqlUtil.CreateParameter(nameof(offset), offset),
            ]);
        }
    }
}
