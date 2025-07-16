using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    /// <summary>
    /// サムネイルT更新
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ThumbnailUpdateSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
UPDATE t_thumbnail
   SET thumbnail_id          = :thumbnail_id
      ,thumbnail_start_point = :thumbnail_start_point
      ,thumbnail_size        = :thumbnail_size
      ,thumbnail_width       = :thumbnail_width
      ,thumbnail_height      = :thumbnail_height
      ,source_width          = :source_width
      ,source_height         = :source_height
      ,file_update_date      = :file_update_date
 WHERE file_path = :file_path
";

        public ThumbnailUpdateSql(
            string filePath,
            int thumbnailID,
            int thumbnailStartPoint,
            int thumbnailSize,
            int thumbnailWidth,
            int thumbnailHeight,
            int sourceWidth,
            int sourceHeight,
            DateTime fileUpdateDate)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            base.Parameters = [
                SqlUtil.CreateParameter("file_path", filePath),
                SqlUtil.CreateParameter("thumbnail_id", thumbnailID),
                SqlUtil.CreateParameter("thumbnail_start_point", thumbnailStartPoint),
                SqlUtil.CreateParameter("thumbnail_size", thumbnailSize),
                SqlUtil.CreateParameter("thumbnail_width", thumbnailWidth),
                SqlUtil.CreateParameter("thumbnail_height", thumbnailHeight),
                SqlUtil.CreateParameter("source_width", sourceWidth),
                SqlUtil.CreateParameter("source_height", sourceHeight),
                SqlUtil.CreateParameter("file_update_date", fileUpdateDate)
            ];
        }
    }
}
