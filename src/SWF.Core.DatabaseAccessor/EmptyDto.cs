using System.Data;

namespace SWF.Core.DatabaseAccessor
{
    public sealed class EmptyDto
        : IDto
    {
        public void Read(IDataReader reader) =>
            ArgumentNullException.ThrowIfNull(reader, nameof(reader));
    }
}
