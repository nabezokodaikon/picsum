namespace SWF.Core.DatabaseAccessor
{
    public interface IDB
        : IDisposable
    {
        IConnection Connect();
        IConnection ConnectWithTransaction();
    }
}
