using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// サムネイルIDを取得します。
    /// </summary>
    public sealed class ReadThumbnailIDSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
SELECT thumbnail_id
  FROM m_thumbnail_id;
";

        public ReadThumbnailIDSql()
            : base(SQL_TEXT)
        {

        }
    }
}
