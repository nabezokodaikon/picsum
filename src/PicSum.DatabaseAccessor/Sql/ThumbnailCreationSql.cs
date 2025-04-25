using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    /// <summary>
    /// サムネイルT作成
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
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

            base.ParameterList.AddRange(
            [SqlUtil.CreateParameter("file_path", filePath),
                SqlUtil.CreateParameter("thumbnail_id", thumbnailID),
                SqlUtil.CreateParameter("thumbnail_start_point", thumbnailStartPoint),
                SqlUtil.CreateParameter("thumbnail_size", thumbnailSize),
                SqlUtil.CreateParameter("thumbnail_width", thumbnailWidth),
                SqlUtil.CreateParameter("thumbnail_height", thumbnailHeight),
                SqlUtil.CreateParameter("source_width", sourceWidth),
                SqlUtil.CreateParameter("source_height", sourceHeight),
                SqlUtil.CreateParameter("file_update_date", fileUpdateDate)]);
        }
    }
}
