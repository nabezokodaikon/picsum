using PicSum.Core.Data.DatabaseAccessor;
using System;
using System.Data;

namespace PicSum.Data.DatabaseAccessor.Dto
{
    public sealed class MFileDto
        : IDto
    {
        public long FileID { get; private set; }
        public string FilePath { get; private set; }

        public void Read(IDataReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            this.FileID = (long)reader["file_id"];
            this.FilePath = (string)reader["file_path"];
        }
    }
}
