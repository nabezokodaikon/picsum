using System.Data;

namespace PicSum.Core.DatabaseAccessor
{
    /// <summary>
    /// Dtoインターフェース
    /// </summary>
    public interface IDto
    {
        void Read(IDataReader reader);
    }
}
