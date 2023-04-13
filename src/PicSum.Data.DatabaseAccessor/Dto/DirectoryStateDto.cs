using System;
using System.Data;
using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Dto
{
    /// <summary>
    /// フォルダ状態DTO
    /// </summary>
    public class DirectoryStateDto : IDto
    {
        private string _directoryPath;
        private int _sortTypeId;
        private bool _isAscending;
        private string _selectedFilePath;

        public string DirectoryPath
        {
            get
            {
                return _directoryPath;
            }
        }

        public int SortTypeId
        {
            get
            {
                return _sortTypeId;
            }
        }

        public bool IsAscending
        {
            get
            {
                return _isAscending;
            }
        }

        public string SelectedFilePath
        {
            get
            {
                return _selectedFilePath;
            }
        }

        public void Read(IDataReader reader)
        {
            _directoryPath = (string)reader["directory_path"];
            _sortTypeId = (int)(long)reader["sort_type_id"];
            _isAscending = (bool)reader["is_ascending"];
            if (!reader["selected_file_path"].Equals(DBNull.Value))
            {
                _selectedFilePath = (string)reader["selected_file_path"];
            }
            else
            {
                _selectedFilePath = string.Empty;
            }
        }
    }
}
