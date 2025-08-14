namespace SWF.Core.DatabaseAccessor
{
    public interface IDatabase
        : IDisposable
    {
        IDatabaseConnection Connect();
        IDatabaseConnection ConnectWithTransaction();
    }
}
