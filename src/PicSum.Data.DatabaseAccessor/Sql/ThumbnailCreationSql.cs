using PicSum.Core.Data.DatabaseAccessor;
using System;
using System.Data;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// サムネイルT作成
    /// </summary>
    public sealed class ThumbnailCreationSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
INSERT INTO t_thumbnail (
     file_path
    ,thumbnail_id
    ,thumbnail_start_point
    ,thumbnail_size
    ,thumbnail_width
    ,thumbnail_height
    ,source_width
    ,source_height
    ,file_update_date
) VALUES (
     :file_path
    ,:thumbnail_id
    ,:thumbnail_start_point
    ,:thumbnail_size
    ,:thumbnail_width
    ,:thumbnail_height
    ,:source_width
    ,:source_height
    ,:file_update_date
)
";

        public ThumbnailCreationSql(
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

            base.ParameterList.AddRange(new IDbDataParameter[]
            { SqlParameterUtil.CreateParameter("file_path", filePath),
              SqlParameterUtil.CreateParameter("thumbnail_id", thumbnailID),
              SqlParameterUtil.CreateParameter("thumbnail_start_point", thumbnailStartPoint),
              SqlParameterUtil.CreateParameter("thumbnail_size", thumbnailSize),
              SqlParameterUtil.CreateParameter("thumbnail_width", thumbnailWidth),
              SqlParameterUtil.CreateParameter("thumbnail_height", thumbnailHeight),
              SqlParameterUtil.CreateParameter("source_width", sourceWidth),
              SqlParameterUtil.CreateParameter("source_height", sourceHeight),
              SqlParameterUtil.CreateParameter("file_update_date", fileUpdateDate) });
        }
    }
}
