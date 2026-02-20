using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using System.Data;

namespace PicSum.DatabaseAccessor.Dto
{
    /// <summary>
    /// フォルダ表示履歴DTO
    /// </summary>
    public sealed class DirectoryViewHistoryDto
        : IDto
    {
        public string DirectoryPath { get; private set; } = string.Empty;
        public DateTime ViewDate { get; private set; } = DateTimeExtensions.EMPTY;

        public void Read(IDataReader reader)
        {
            ArgumentNullException.ThrowIfNull(reader, nameof(reader));

            this.DirectoryPath = (string)reader["file_path"];
            this.ViewDate = DateTime.FromBinary((long)reader["view_date_ticks"]);
        }
    }
}
