using PicSum.Core.Data.DatabaseAccessor;
using System;
using System.Data;

namespace PicSum.Data.DatabaseAccessor.Dto
{
    /// <summary>
    /// 単一ファイル情報DTO
    /// </summary>
    public sealed class FileInfoDto
        : IDto
    {
        public string FilePath { get; private set; }
        public int Rating { get; private set; }

        public void Read(IDataReader reader)
        {
            ArgumentNullException.ThrowIfNull(reader, nameof(reader));

            this.FilePath = (string)reader["file_path"];
            this.Rating = (int)(long)reader["rating"];
        }
    }
}
