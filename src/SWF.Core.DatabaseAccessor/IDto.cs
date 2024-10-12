using System.Data;

namespace SWF.Core.DatabaseAccessor
{
    /// <summary>
    /// Dtoインターフェース
    /// </summary>
    public interface IDto
    {
        void Read(IDataReader reader);
    }
}
