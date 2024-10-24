using SWF.Core.DatabaseAccessor;
using System.Data;

namespace PicSum.DatabaseAccessor.Dto
{
    public struct TagInfoDto
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
