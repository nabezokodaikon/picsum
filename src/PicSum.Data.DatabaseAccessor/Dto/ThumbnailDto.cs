using System;
using System.Data;
using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Dto
{
    /// <summary>
    /// サムネイルDTO
    /// </summary>
    public class ThumbnailDto : IDto
    {
        private string _filePath;
        private byte[] _thumbnailBuffer;
        private int _thumbnailWidth;
        private int _thumbnailHeight;
        private int _sourceWidth;
        private int _sourceHeight;
        private DateTime _fileUpdatedate;

        public string FilePath
        {
            get
            {
                return _filePath;
            }
        }

        public byte[] ThumbnailBuffer
        {
            get
            {
                return _thumbnailBuffer;
            }
        }

        public int ThumbnailWidth
        {
            get
            {
                return _thumbnailWidth;
            }
        }

        public int ThumbnailHeight
        {
            get
            {
                return _thumbnailHeight;
            }
        }

        public int SourceWidth
        {
            get
            {
                return _sourceWidth;
            }
        }

        public int SourceHeight
        {
            get
            {
                return _sourceHeight;
            }
        }

        public DateTime FileUpdatedate
        {
            get
            {
                return _fileUpdatedate;
            }
        }

        public void Read(IDataReader reader)
        {
            _filePath = (string)reader["file_path"];
            _thumbnailBuffer = (byte[])reader["thumbnail_buffer"];
            _thumbnailWidth = (int)(long)reader["thumbnail_width"];
            _thumbnailHeight = (int)(long)reader["thumbnail_height"];
            _sourceWidth = (int)(long)reader["source_width"];
            _sourceHeight = (int)(long)reader["source_height"];
            _fileUpdatedate = (DateTime)reader["file_update_date"];
        }
    }
}
