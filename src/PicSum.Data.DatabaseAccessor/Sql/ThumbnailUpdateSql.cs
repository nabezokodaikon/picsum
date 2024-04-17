using PicSum.Core.Data.DatabaseAccessor;
using System;
using System.Data;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// サムネイルT更新
    /// </summary>
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

            base.ParameterList.AddRange(
            [ SqlParameterUtil.CreateParameter("file_path", filePath),
              SqlParameterUtil.CreateParameter("thumbnail_id", thumbnailID),
              SqlParameterUtil.CreateParameter("thumbnail_start_point", thumbnailStartPoint),
              SqlParameterUtil.CreateParameter("thumbnail_size", thumbnailSize),
              SqlParameterUtil.CreateParameter("thumbnail_width", thumbnailWidth),
              SqlParameterUtil.CreateParameter("thumbnail_height", thumbnailHeight),
              SqlParameterUtil.CreateParameter("source_width", sourceWidth),
              SqlParameterUtil.CreateParameter("source_height", sourceHeight),
              SqlParameterUtil.CreateParameter("file_update_date", fileUpdateDate) ]);
        }
    }
}
