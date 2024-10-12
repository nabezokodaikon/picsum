using SWF.Core.DatabaseAccessor;
using System;
using System.Data;

namespace PicSum.DatabaseAccessor.Dto
{
    public sealed class MFileDto
        : IDto
    {
        public long FileID { get; private set; }
        public string FilePath { get; private set; }

        public void Read(IDataReader reader)
        {
            ArgumentNullException.ThrowIfNull(reader, nameof(reader));

            this.FileID = (long)reader["file_id"];
            this.FilePath = (string)reader["file_path"];
        }
    }
}
