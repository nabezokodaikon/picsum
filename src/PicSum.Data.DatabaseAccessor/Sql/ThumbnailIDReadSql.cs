using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// サムネイルIDを取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
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
