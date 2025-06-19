using System.Data;
using System.Data.SQLite;
using System.Runtime.Versioning;

namespace SWF.Core.DatabaseAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class DBConnection
        : IDBConnection
    {
        private bool _disposed = false;
        private readonly Lock _lockObject;
        private readonly SQLiteConnection _connection;
        private readonly SQLiteTransaction? _transaction = null;
        private readonly bool _isDispose;
        private bool _isCommitted = false;

        public DBConnection(Lock lockObject, string filePath, bool isTransaction)
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

        public DBConnection(Lock lockObject, SQLiteConnection connection, bool isTransaction)
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
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
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
                    this._connection.Close();
                    this._connection.Dispose();
                }

                this._lockObject.Exit();
            }

            this._disposed = true;
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

            if (this._transaction == null)
            {
                throw new InvalidOperationException("トランザクションが開始されていません。");
            }

            using (var cmd = this._connection.CreateCommand())
            {
                cmd.CommandText = sql.GetExecuteSql();

                if (sql.ParameterList.Count > 0)
                {
                    cmd.Parameters.AddRange(sql.ParameterList.ToArray());
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

            using (var cmd = this._connection.CreateCommand())
            {
                cmd.CommandText = sql.GetExecuteSql();

                if (sql.ParameterList.Count > 0)
                {
                    cmd.Parameters.AddRange(sql.ParameterList.ToArray());
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

            using (var cmd = this._connection.CreateCommand())
            {
                cmd.CommandText = sql.GetExecuteSql();

                if (sql.ParameterList.Count > 0)
                {
                    cmd.Parameters.AddRange(sql.ParameterList.ToArray());
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

            using (var cmd = this._connection.CreateCommand())
            {
                cmd.CommandText = sql.GetExecuteSql();

                if (sql.ParameterList.Count > 0)
                {
                    cmd.Parameters.AddRange(sql.ParameterList.ToArray());
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
