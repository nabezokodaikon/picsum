namespace SWF.Core.DatabaseAccessor
{
    public interface ITransaction
        : IDisposable
    {
        public void Commit();
    }
}
