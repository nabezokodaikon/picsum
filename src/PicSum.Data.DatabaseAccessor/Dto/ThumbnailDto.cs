using PicSum.Core.DatabaseAccessor;
using System;
using System.Data;

namespace PicSum.Data.DatabaseAccessor.Dto
{
    /// <summary>
    /// サムネイルDTO
    /// </summary>
    public sealed class ThumbnailDto
        : IDto
    {
        public string FilePath { get; private set; }
        public int ThumbnailID { get; private set; }
        public int ThumbnailStartPoint { get; private set; }
        public int ThumbnailSize { get; private set; }
        public int ThumbnailWidth { get; private set; }
        public int ThumbnailHeight { get; private set; }
        public int SourceWidth { get; private set; }
        public int SourceHeight { get; private set; }
        public DateTime FileUpdatedate { get; private set; }

        public void Read(IDataReader reader)
        {
            ArgumentNullException.ThrowIfNull(reader, nameof(reader));

            this.FilePath = (string)reader["file_path"];
            this.ThumbnailID = (int)(long)reader["thumbnail_id"];
            this.ThumbnailStartPoint = (int)(long)reader["thumbnail_start_point"];
            this.ThumbnailSize = (int)(long)reader["thumbnail_size"];
            this.ThumbnailWidth = (int)(long)reader["thumbnail_width"];
            this.ThumbnailHeight = (int)(long)reader["thumbnail_height"];
            this.SourceWidth = (int)(long)reader["source_width"];
            this.SourceHeight = (int)(long)reader["source_height"];
            this.FileUpdatedate = (DateTime)reader["file_update_date"];
        }
    }
}
