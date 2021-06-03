using System.Data;
using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Dto
{
    /// <summary>
    /// 単一値DTO
    /// </summary>
    public class SingleValueDto<T> : IDto
    {
        private T _value;

        public T Value
        {
            get
            {
                return _value;
            }
        }

        public void Read(IDataReader reader)
        {
            _value = (T)reader[0];
        }
    }
}
