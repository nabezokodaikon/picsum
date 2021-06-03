using System;
using System.Data;
using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Dto
{
    /// <summary>
    /// フォルダ状態DTO
    /// </summary>
    public class FolderStateDto : IDto
    {
        private string _folderPath;
        private int _sortTypeId;
        private bool _isAscending;
        private string _selectedFilePath;

        public string FolderPath
        {
            get
            {
                return _folderPath;
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
            _folderPath = (string)reader["folder_path"];
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
