namespace SWF.Core.DatabaseAccessor
{
    public interface IDatabase
        : IAsyncDisposable
    {
        Task<IDatabaseConnection> Connect();
        Task<IDatabaseConnection> ConnectWithTransaction();
    }
}
