namespace SWF.Core.DatabaseAccessor
{
    public interface IDatabase
        : IAsyncDisposable
    {
        ValueTask<IDatabaseConnection> Connect();
        ValueTask<IDatabaseConnection> ConnectWithTransaction();
    }
}
