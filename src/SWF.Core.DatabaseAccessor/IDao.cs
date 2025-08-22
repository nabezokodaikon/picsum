namespace SWF.Core.DatabaseAccessor
{
    public interface IDao
        : IDisposable
    {
        ValueTask<IConnection> Connect();
        ValueTask<IConnection> ConnectWithTransaction();
    }
}
