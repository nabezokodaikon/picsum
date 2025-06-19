namespace SWF.Core.DatabaseAccessor
{
    public interface IDB
        : IDisposable
    {
        IDBConnection Connect();
        IDBConnection ConnectWithTransaction();
    }
}
