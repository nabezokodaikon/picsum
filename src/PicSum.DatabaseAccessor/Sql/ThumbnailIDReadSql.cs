using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    /// <summary>
    /// サムネイルIDを取得します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ThumbnailIDReadSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
SELECT thumbnail_id
  FROM m_thumbnail_id;
";

        public ThumbnailIDReadSql()
            : base(SQL_TEXT)
        {

        }
    }
}
