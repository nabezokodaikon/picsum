using PicSum.Core.Data.DatabaseAccessor;
using System.Data;

namespace PicSum.Data.DatabaseAccessor.Dto
{
    /// <summary>
    /// 単一値DTO
    /// </summary>
    public sealed class SingleValueDto<T> 
        : IDto
    {
        public T Value { get; private set; }

        public void Read(IDataReader reader)
        {
            this.Value = (T)reader[0];
        }
    }
}
