namespace SWF.Core.DatabaseAccessor
{
    public interface IConnection
        : IAsyncDisposable
    {
        ValueTask Commit();
        ValueTask<bool> Update(SqlBase sql);
        ValueTask<TDto[]> ReadList<TDto>(SqlBase<TDto> sql)
            where TDto : IDto, new();
        ValueTask<TDto?> ReadLine<TDto>(SqlBase<TDto> sql)
            where TDto : class, IDto, new();
        ValueTask<T?> ReadValue<T>(SqlBase sql);
    }
}