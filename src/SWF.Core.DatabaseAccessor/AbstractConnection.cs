using SWF.Core.Job;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace SWF.Core.DatabaseAccessor
{
    public abstract class AbstractConnection
    {
        internal protected bool IsOccursedException { get; set; } = false;
        internal protected SQLiteConnection? Connection { get; set; } = null;

        public async ValueTask<bool> Update(SqlBase sql)
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            if (this.Connection is null)
            {
                throw new InvalidOperationException("コネクションが設定されていません。");
            }

            using (var cmd = this.Connection.CreateCommand())
            {
#pragma warning disable CA2100
                cmd.CommandText = sql.GetExecuteSql();
#pragma warning restore CA2100

                if (sql.Parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(sql.Parameters);
                }

                try
                {
                    var result = await cmd.ExecuteNonQueryAsync().False();
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
                catch (DbException)
                {
                    this.IsOccursedException = true;
                    throw;
                }
            }
        }

        public async ValueTask<TDto[]> ReadList<TDto>(SqlBase<TDto> sql)
            where TDto : IDto, new()
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            if (this.Connection is null)
            {
                throw new InvalidOperationException("コネクションが設定されていません。");
            }

            using (var cmd = this.Connection.CreateCommand())
            {
#pragma warning disable CA2100
                cmd.CommandText = sql.GetExecuteSql();
#pragma warning restore CA2100

                if (sql.Parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(sql.Parameters);
                }

                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.Default).False())
                    {
                        if (reader.HasRows)
                        {
                            var list = new List<TDto>();

                            while (await reader.ReadAsync().False())
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
                catch (DbException)
                {
                    this.IsOccursedException = true;
                    throw;
                }
            }
        }

        public async ValueTask<TDto?> ReadLine<TDto>(SqlBase<TDto> sql)
                 where TDto : class, IDto, new()
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            if (this.Connection is null)
            {
                throw new InvalidOperationException("コネクションが設定されていません。");
            }

            using (var cmd = this.Connection.CreateCommand())
            {
#pragma warning disable CA2100
                cmd.CommandText = sql.GetExecuteSql();
#pragma warning restore CA2100

                if (sql.Parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(sql.Parameters);
                }

                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow).False())
                    {
                        if (reader.HasRows)
                        {
                            await reader.ReadAsync().False();
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
                catch (DbException)
                {
                    this.IsOccursedException = true;
                    throw;
                }
            }
        }

        public async ValueTask<T?> ReadValue<T>(SqlBase sql)
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            if (this.Connection is null)
            {
                throw new InvalidOperationException("コネクションが設定されていません。");
            }

            using (var cmd = this.Connection.CreateCommand())
            {
#pragma warning disable CA2100
                cmd.CommandText = sql.GetExecuteSql();
#pragma warning restore CA2100

                if (sql.Parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(sql.Parameters);
                }

                try
                {
                    var result = await cmd.ExecuteScalarAsync().False();
                    if (result != null && result != DBNull.Value)
                    {
                        return (T)result;
                    }
                    else
                    {
                        return default;
                    }
                }
                catch (DbException)
                {
                    this.IsOccursedException = true;
                    throw;
                }
            }
        }
    }
}