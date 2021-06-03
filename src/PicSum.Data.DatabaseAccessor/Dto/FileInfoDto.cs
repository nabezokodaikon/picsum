using System;
using System.Data;
using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Dto
{
    /// <summary>
    /// 単一ファイル情報DTO
    /// </summary>
    public class FileInfoDto : IDto
    {
        private string _filePath;
        private int _rating;
        private int _viewCount;
        private Nullable<DateTime> _lastViewDate;

        public string FilePath
        {
            get
            {
                return _filePath;
            }
        }

        public int Rating
        {
            get
            {
                return _rating;
            }
        }

        public int ViewCount
        {
            get
            {
                return _viewCount;
            }
        }

        public Nullable<DateTime> LastViewDate
        {
            get
            {
                return _lastViewDate;
            }
        }

        public void Read(IDataReader reader)
        {
            _filePath = (string)reader["file_path"];
            _rating = (int)(long)reader["rating"];
            _viewCount = (int)(long)reader["view_count"];
            if (!reader["last_view_date"].Equals(DBNull.Value))
            {
                _lastViewDate = DateTime.Parse((string)reader["last_view_date"]);
            }
            else
            {
                _lastViewDate = null;
            }
        }
    }
}
