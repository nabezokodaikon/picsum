using SWF.Core.DatabaseAccessor;
using System.Data;

namespace PicSum.DatabaseAccessor.Dto
{
    /// <summary>
    /// 単一値DTO
    /// </summary>
    public sealed class SingleValueDto<T>
        : IDto
    {
        public T? Value { get; private set; }

        public void Read(IDataReader reader)
        {
            ArgumentNullException.ThrowIfNull(reader, nameof(reader));
            this.Value = (T)reader[0];
        }
    }
}
