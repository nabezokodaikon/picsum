using SWF.Core.DatabaseAccessor;
using System;
using System.Data;

namespace PicSum.DatabaseAccessor.Dto
{
    /// <summary>
    /// フォルダ表示履歴DTO
    /// </summary>
    public class DirectoryViewHistoryDto
        : IDto
    {
        public string DirectoryPath { get; private set; }
        public DateTime ViewDate { get; private set; }

        public void Read(IDataReader reader)
        {
            ArgumentNullException.ThrowIfNull(reader, nameof(reader));

            this.DirectoryPath = (string)reader["file_path"];
            this.ViewDate = (DateTime)reader["view_date"];
        }
    }
}
