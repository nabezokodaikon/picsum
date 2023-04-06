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

        public void Read(IDataReader reader)
        {
            _filePath = (string)reader["file_path"];
            _rating = (int)(long)reader["rating"];
        }
    }
}
