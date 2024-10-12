using PicSum.Core.DatabaseAccessor;
using System;
using System.Data;

namespace PicSum.Data.DatabaseAccessor.Dto
{
    /// <summary>
    /// 複数ファイル情報DTO
    /// </summary>
    public sealed class FileTagDto
        : IDto
    {
        public string Tag { get; private set; }
        public bool IsAll { get; private set; }

        public void Read(IDataReader reader)
        {
            ArgumentNullException.ThrowIfNull(reader, nameof(reader));

            this.Tag = (string)reader["tag"];
            this.IsAll = bool.Parse(reader["is_all"].ToString());
        }
    }
}
