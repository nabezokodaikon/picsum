namespace SWF.Core.DatabaseAccessor
{
    public interface IDao
        : IDisposable
    {
        public void Connect(IConnection connection);
        public ITransaction BeginTransaction();
        public bool Update(SqlBase sql);
        public IList<TDto> ReadList<TDto>(SqlBase<TDto> sql)
            where TDto : IDto, new();
        public TDto? ReadLine<TDto>(SqlBase<TDto> sql)
            where TDto : IDto, new();
        public T? ReadValue<T>(SqlBase sql);
    }
}
