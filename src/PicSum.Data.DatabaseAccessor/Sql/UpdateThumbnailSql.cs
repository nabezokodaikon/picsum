using PicSum.Core.Data.DatabaseAccessor;
using System;
using System.Data;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// サムネイルT更新
    /// </summary>
    public sealed class UpdateThumbnailSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
UPDATE t_thumbnail
   SET thumbnail_buffer = :thumbnail_buffer
      ,thumbnail_width  = :thumbnail_width
      ,thumbnail_height = :thumbnail_height
      ,source_width     = :source_width
      ,source_height    = :source_height
      ,file_update_date = :file_update_date
 WHERE file_path = :file_path
";

        public UpdateThumbnailSql(string filePath, byte[] thumbnailBuffer, int thumbnailWidth, int thumbnailHeight, int sourceWidth, int sourceHeight, DateTime fileUpdateDate)
            : base(SQL_TEXT)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (thumbnailBuffer == null)
            {
                throw new ArgumentNullException(nameof(thumbnailBuffer));
            }

            base.ParameterList.AddRange(new IDbDataParameter[]
            { SqlParameterUtil.CreateParameter("file_path", filePath),
              SqlParameterUtil.CreateParameter("thumbnail_buffer", thumbnailBuffer),
              SqlParameterUtil.CreateParameter("thumbnail_width", thumbnailWidth),
              SqlParameterUtil.CreateParameter("thumbnail_height", thumbnailHeight),
              SqlParameterUtil.CreateParameter("source_width", sourceWidth),
              SqlParameterUtil.CreateParameter("source_height", sourceHeight),
              SqlParameterUtil.CreateParameter("file_update_date", fileUpdateDate) });
        }
    }
}
