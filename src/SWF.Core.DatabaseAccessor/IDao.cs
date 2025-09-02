namespace SWF.Core.DatabaseAccessor
{
    public interface IDao
        : IDisposable
    {
        IConnection Connect();
        IConnection ConnectWithTransaction();
    }
}
