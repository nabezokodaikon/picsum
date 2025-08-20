using SWF.Core.Base;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace SWF.Core.DatabaseAccessor
{

    internal sealed class DatabaseConnection
        : IDatabaseConnection
    {
        private bool _disposed = false;
#pragma warning disable CA2213
        private SemaphoreSlim? _lockObject = null;
#pragma warning restore CA2213
        private SQLiteConnection? _connection = null;
        private DbTransaction? _transaction = null;
        private bool _isDispose;
        private bool _isCommitted = false;

        public async ValueTask Initialize(SemaphoreSlim lockObject, string filePath, bool isTransaction)
        {
            ArgumentNullException.ThrowIfNull(lockObject, nameof(lockObject));
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this._lockObject = lockObject;
            this._connection = new SQLiteConnection($"Data Source={filePath}");
            await this._connection.OpenAsync().WithConfig();

            if (isTransaction)
            {
                this._transaction = await this._connection.BeginTransactionAsync().WithConfig();
            }

            this._isDispose = true;
        }

        public async ValueTask Initialize(SemaphoreSlim lockObject, SQLiteConnection connection, bool isTransaction)
        {
            ArgumentNullException.ThrowIfNull(lockObject, nameof(lockObject));
            ArgumentNullException.ThrowIfNull(connection, nameof(connection));

            this._lockObject = lockObject;
            this._connection = connection;

            if (isTransaction)
            {
                this._transaction = await this._connection.BeginTransactionAsync().WithConfig();
            }

            this._isDispose = false;
        }

        public async ValueTask DisposeAsync()
        {
            if (this._disposed)
            {
                return;
            }

            if (this._transaction != null)
            {
                if (!this._isCommitted)
                {
                    await this._transaction.RollbackAsync().WithConfig();
                }

                await this._transaction.DisposeAsync().WithConfig();
            }

            if (this._isDispose)
            {
                if (this._connection is not null)
                {
                    await this._connection.DisposeAsync().WithConfig();
                }
            }

            this._lockObject?.Release();

            this._disposed = true;

            GC.SuppressFinalize(this);
        }

        public async ValueTask Commit()
        {
            if (this._transaction == null)
            {
                throw new InvalidOperationException("トランザクションが開始されていません。");
            }

            await this._transaction.CommitAsync().WithConfig();
            this._isCommitted = true;
        }

        public async ValueTask<bool> Update(SqlBase sql)
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            if (this._connection is null)
            {
                throw new InvalidOperationException("コネクションが設定されていません。");
            }

            using (var cmd = this._connection.CreateCommand())
            {
                cmd.CommandText = sql.GetExecuteSql();

                if (sql.Parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(sql.Parameters);
                }

                var result = await cmd.ExecuteNonQueryAsync().WithConfig();
                if (result > 0)
                {
                    // 更新されたレコードが存在するため、Trueを返します。
                    return true;
                }
                else
                {
                    // 更新されたレコードが存在しないため、Falseを返します。
                    return false;
                }
            }
        }

        public async ValueTask<TDto[]> ReadList<TDto>(SqlBase<TDto> sql)
            where TDto : IDto, new()
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            if (this._connection is null)
            {
                throw new InvalidOperationException("コネクションが設定されていません。");
            }

            using (var cmd = this._connection.CreateCommand())
            {
                cmd.CommandText = sql.GetExecuteSql();

                if (sql.Parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(sql.Parameters);
                }

                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.Default).WithConfig())
                {
                    if (reader.HasRows)
                    {
                        var list = new List<TDto>();

                        while (await reader.ReadAsync().WithConfig())
                        {
                            var dto = new TDto();
                            dto.Read(reader);
                            list.Add(dto);
                        }

                        return [.. list];
                    }
                    else
                    {
                        return [];
                    }
                }
            }
        }

        public async ValueTask<TDto?> ReadLine<TDto>(SqlBase<TDto> sql)
                 where TDto : class, IDto, new()
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            if (this._connection is null)
            {
                throw new InvalidOperationException("コネクションが設定されていません。");
            }

            using (var cmd = this._connection.CreateCommand())
            {
                cmd.CommandText = sql.GetExecuteSql();

                if (sql.Parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(sql.Parameters);
                }

                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow).WithConfig())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync().WithConfig();
                        var dto = new TDto();
                        dto.Read(reader);
                        return dto;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public async ValueTask<T?> ReadValue<T>(SqlBase sql)
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            if (this._connection is null)
            {
                throw new InvalidOperationException("コネクションが設定されていません。");
            }

            using (var cmd = this._connection.CreateCommand())
            {
                cmd.CommandText = sql.GetExecuteSql();

                if (sql.Parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(sql.Parameters);
                }

                var result = await cmd.ExecuteScalarAsync().WithConfig();
                if (result != null && result != DBNull.Value)
                {
                    return (T)result;
                }
                else
                {
                    return default;
                }
            }
        }
    }
}
