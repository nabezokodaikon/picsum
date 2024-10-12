using PicSum.Core.DatabaseAccessor;
using System;
using System.Data;

namespace PicSum.Data.DatabaseAccessor.Dto
{
    public sealed class TagInfoDto
        : IDto
    {
        public string FilePath { get; private set; }
        public string Tag { get; private set; }

        public void Read(IDataReader reader)
        {
            ArgumentNullException.ThrowIfNull(reader, nameof(reader));

            this.FilePath = (string)reader["file_path"];
            this.Tag = (string)reader["tag"];
        }
    }
}
