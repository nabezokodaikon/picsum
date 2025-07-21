using SWF.Core.DatabaseAccessor;
using System.Data;

namespace PicSum.DatabaseAccessor.Dto
{
    public sealed class FavoriteDirecotryDto
        : IDto
    {
        public string DirectoryPath { get; private set; } = string.Empty;
        public long ViewCount { get; private set; } = 0;

        public void Read(IDataReader reader)
        {
            ArgumentNullException.ThrowIfNull(reader, nameof(reader));

            this.DirectoryPath = (string)reader["file_path"];
            this.ViewCount = (long)reader["cnt"];
        }
    }
}
