using System.Data;

namespace SWF.Core.DatabaseAccessor
{
    public struct EmptyDto
        : IDto
    {
        public readonly void Read(IDataReader reader) =>
            ArgumentNullException.ThrowIfNull(reader, nameof(reader));
    }
}
