namespace SWF.Core.DatabaseAccessor
{
    public interface IDatabase
        : IDisposable
    {
        ValueTask<IDatabaseConnection> Connect();
        ValueTask<IDatabaseConnection> ConnectWithTransaction();
    }
}
