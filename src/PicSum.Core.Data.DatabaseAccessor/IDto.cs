using System.Data;

namespace PicSum.Core.Data.DatabaseAccessor
{
    /// <summary>
    /// Dtoインターフェース
    /// </summary>
    public interface IDto
    {
        void Read(IDataReader reader);
    }
}
