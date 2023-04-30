using PicSum.Core.Data.DatabaseAccessor;
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
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            this.FilePath = (string)reader["file_path"];
            this.Tag = (string)reader["tag"];
        }
    }
}
