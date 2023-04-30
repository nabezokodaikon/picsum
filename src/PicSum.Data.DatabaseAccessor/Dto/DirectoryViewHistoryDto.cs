using PicSum.Core.Data.DatabaseAccessor;
using System;
using System.Data;

namespace PicSum.Data.DatabaseAccessor.Dto
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
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            this.DirectoryPath = (string)reader["file_path"];
            this.ViewDate = (DateTime)reader["view_date"];
        }
    }
}
