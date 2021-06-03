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
    public class FolderViewHistoryDto : IDto
    {
        private string _folderPath;
        private DateTime _viewDate;

        public string FolderPath
        {
            get
            {
                return _folderPath;
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
            _folderPath = (string)reader["file_path"];
            _viewDate = (DateTime)reader["view_date"];
        }
    }
}
