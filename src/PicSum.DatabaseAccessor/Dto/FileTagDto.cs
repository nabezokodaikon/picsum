using SWF.Core.DatabaseAccessor;
using System.Data;

namespace PicSum.DatabaseAccessor.Dto
{
    /// <summary>
    /// 複数ファイル情報DTO
    /// </summary>
    public sealed class FileTagDto
        : IDto
    {
        public string Tag { get; private set; } = string.Empty;
        public bool IsAll { get; private set; }

        public void Read(IDataReader reader)
        {
            ArgumentNullException.ThrowIfNull(reader, nameof(reader));

            this.Tag = (string)reader["tag"];
            this.IsAll = bool.Parse((string)reader["is_all"]);
        }
    }
}
