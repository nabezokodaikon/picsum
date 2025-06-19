namespace SWF.Core.DatabaseAccessor
{
    public interface IDBConnection
        : IDisposable
    {
        void Commit();
        bool Update(SqlBase sql);
        TDto[] ReadList<TDto>(SqlBase<TDto> sql)
            where TDto : IDto, new();
        TDto? ReadLine<TDto>(SqlBase<TDto> sql)
            where TDto : class, IDto, new();
        T? ReadValue<T>(SqlBase sql);
    }
}
