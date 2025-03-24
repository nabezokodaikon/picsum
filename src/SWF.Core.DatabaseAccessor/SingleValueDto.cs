using System.Data;

namespace SWF.Core.DatabaseAccessor
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

        public T GetValueOrDefault(T defaultValue)
        {
            ArgumentNullException.ThrowIfNull((T)defaultValue, nameof(defaultValue));

            if (this.Value != null)
            {
                return this.Value;
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
