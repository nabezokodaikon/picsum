using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace SWF.Core.DatabaseAccessor
{

    internal sealed class Connection
        : IConnection
    {
        private bool _disposed = false;
#pragma warning disable CA2213
        private Lock? _lockObject = null;
        private SQLiteConnection? _connection = null;
#pragma warning restore CA2213
        private DbTransaction? _transaction = null;
        private bool _isDispose;
        private bool _isCommitted = false;

        public void Initialize(Lock lockObject, string filePath, bool isTransaction)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this._lockObject = lockObject;
            this._connection = new SQLiteConnection($"Data Source={filePath}");
            this._connection.Open();

            if (isTransaction)
            {
                this._transaction = this._connection.BeginTransaction();
            }

            this._isDispose = true;
        }

        public void Initialize(Lock lockObject, SQLiteConnection connection, bool isTransaction)
        {
            ArgumentNullException.ThrowIfNull(connection, nameof(connection));

            this._lockObject = lockObject;
            this._connection = connection;

            if (isTransaction)
            {
                this._transaction = this._connection.BeginTransaction();
            }

            this._isDispose = false;
        }

        public void Dispose()
        {
            if (this._disposed)
            {
                return;
            }

            if (this._transaction != null)
            {
                if (!this._isCommitted)
                {
                    this._transaction.Rollback();
                }

                this._transaction.Dispose();
            }

            if (this._isDispose)
            {
                if (this._connection is not null)
                {
                    this._connection.Dispose();
                }
            }

            this._lockObject?.Exit();

            this._disposed = true;

            GC.SuppressFinalize(this);
        }

        public void Commit()
        {
            if (this._transaction == null)
            {
                throw new InvalidOperationException("トランザクションが開始されていません。");
            }

            this._transaction.Commit();
            this._isCommitted = true;
        }

        public bool Update(SqlBase sql)
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            if (this._connection is null)
            {
                throw new InvalidOperationException("コネクションが設定されていません。");
            }

            using (var cmd = this._connection.CreateCommand())
            {
#pragma warning disable CA2100
                cmd.CommandText = sql.GetExecuteSql();
#pragma warning restore CA2100

                if (sql.Parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(sql.Parameters);
                }

                var result = cmd.ExecuteNonQuery();
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

        public TDto[] ReadList<TDto>(SqlBase<TDto> sql)
            where TDto : IDto, new()
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            if (this._connection is null)
            {
                throw new InvalidOperationException("コネクションが設定されていません。");
            }

            using (var cmd = this._connection.CreateCommand())
            {
#pragma warning disable CA2100
                cmd.CommandText = sql.GetExecuteSql();
#pragma warning restore CA2100

                if (sql.Parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(sql.Parameters);
                }

                using (var reader = cmd.ExecuteReader(CommandBehavior.Default))
                {
                    if (reader.HasRows)
                    {
                        var list = new List<TDto>();

                        while (reader.Read())
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

        public TDto? ReadLine<TDto>(SqlBase<TDto> sql)
                 where TDto : class, IDto, new()
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            if (this._connection is null)
            {
                throw new InvalidOperationException("コネクションが設定されていません。");
            }

            using (var cmd = this._connection.CreateCommand())
            {
#pragma warning disable CA2100
                cmd.CommandText = sql.GetExecuteSql();
#pragma warning restore CA2100

                if (sql.Parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(sql.Parameters);
                }

                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
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

        public T? ReadValue<T>(SqlBase sql)
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            if (this._connection is null)
            {
                throw new InvalidOperationException("コネクションが設定されていません。");
            }

            using (var cmd = this._connection.CreateCommand())
            {
#pragma warning disable CA2100
                cmd.CommandText = sql.GetExecuteSql();
#pragma warning restore CA2100

                if (sql.Parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(sql.Parameters);
                }

                var result = cmd.ExecuteScalar();
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
