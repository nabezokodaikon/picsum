namespace SWF.Core.DatabaseAccessor
{
    public interface IConnection
        : IDisposable
    {
        public string DBFilePath { get; }

        public ITransaction BeginTransaction();
        public void Commit();
        public void Roolback();
        public bool Update(SqlBase sql);
        public TDto[] ReadList<TDto>(SqlBase<TDto> sql)
            where TDto : IDto, new();
        public TDto? ReadLine<TDto>(SqlBase<TDto> sql)
            where TDto : IDto, new();
        public T? ReadValue<T>(SqlBase sql);
    }
}
