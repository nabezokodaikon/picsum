using PicSum.Core.Data.DatabaseAccessor;
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
        public byte[] ThumbnailBuffer { get; private set; }
        public int ThumbnailWidth { get; private set; }
        public int ThumbnailHeight { get; private set; }
        public int SourceWidth { get; private set; }
        public int SourceHeight { get; private set; }
        public DateTime FileUpdatedate { get; private set; }

        public void Read(IDataReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            this.FilePath = (string)reader["file_path"];
            this.ThumbnailBuffer = (byte[])reader["thumbnail_buffer"];
            this.ThumbnailWidth = (int)(long)reader["thumbnail_width"];
            this.ThumbnailHeight = (int)(long)reader["thumbnail_height"];
            this.SourceWidth = (int)(long)reader["source_width"];
            this.SourceHeight = (int)(long)reader["source_height"];
            this.FileUpdatedate = (DateTime)reader["file_update_date"];
        }
    }
}
