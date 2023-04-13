using PicSum.Core.Data.DatabaseAccessor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSum.Data.DatabaseAccessor.Dto
{
    /// <summary>
    /// フォルダ表示履歴DTO
    /// </summary>
    public class DirectoryViewHistoryDto : IDto
    {
        private string _directoryPath;
        private DateTime _viewDate;

        public string DirectoryPath
        {
            get
            {
                return _directoryPath;
            }
        }

        public DateTime ViewDate
        {
            get
            {
                return _viewDate;
            }
        }

        public void Read(IDataReader reader)
        {
            _directoryPath = (string)reader["file_path"];
            _viewDate = (DateTime)reader["view_date"];
        }
    }
}
