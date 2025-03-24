using SWF.Core.DatabaseAccessor;
using System.Data;

namespace PicSum.DatabaseAccessor.Dto
{
    public sealed class TagInfoDto
        : IDto
    {
        public string FilePath { get; private set; } = string.Empty;
        public string Tag { get; private set; } = string.Empty;

        public void Read(IDataReader reader)
        {
            ArgumentNullException.ThrowIfNull(reader, nameof(reader));

            this.FilePath = (string)reader["file_path"];
            this.Tag = (string)reader["tag"];
        }
    }
}
