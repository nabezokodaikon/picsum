using SWF.Core.DatabaseAccessor;
using System;
using System.Data;

namespace PicSum.DatabaseAccessor.Dto
{
    /// <summary>
    /// フォルダ状態DTO
    /// </summary>
    public sealed class DirectoryStateDto
        : IDto
    {
        public string DirectoryPath { get; private set; }
        public int SortTypeId { get; private set; }
        public bool IsAscending { get; private set; }
        public string SelectedFilePath { get; private set; }

        public void Read(IDataReader reader)
        {
            ArgumentNullException.ThrowIfNull(reader, nameof(reader));

            this.DirectoryPath = (string)reader["directory_path"];
            this.SortTypeId = (int)(long)reader["sort_type_id"];
            this.IsAscending = (bool)reader["is_ascending"];
            if (!reader["selected_file_path"].Equals(DBNull.Value))
            {
                this.SelectedFilePath = (string)reader["selected_file_path"];
            }
            else
            {
                this.SelectedFilePath = string.Empty;
            }
        }
    }
}
