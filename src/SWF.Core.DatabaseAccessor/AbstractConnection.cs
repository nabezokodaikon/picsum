using SWF.Core.FileAccessor;
using System.Data;
using System.Data.SQLite;
using System.Runtime.Versioning;

namespace SWF.Core.DatabaseAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public abstract class AbstractConnection
        : IConnection
    {
        private bool _disposed = false;
        private readonly Lock _lockObject = new();
        private SQLiteTransaction? _transaction = null;

        protected readonly string _dbFilePath;
        protected SQLiteConnection? _connection = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dbFilePath">DBファイルパス</param>
        /// <param name="tableCreateSql">テーブル作成SQL</param>
        protected AbstractConnection(string dbFilePath, string tableCreateSql)
        {
            ArgumentException.ThrowIfNullOrEmpty(dbFilePath, nameof(dbFilePath));
            ArgumentException.ThrowIfNullOrEmpty(tableCreateSql, nameof(tableCreateSql));

            if (!FileUtil.IsExistsFile(dbFilePath))
            {
                SQLiteConnection.CreateFile(dbFilePath);
                using (var con = new SQLiteConnection($"Data Source={dbFilePath}"))
                {
                    con.Open();
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = tableCreateSql;
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
            }

            this._dbFilePath = dbFilePath;
        }

        /// <summary>
        /// リソースを解放します。
        /// </summary>
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
                this.Close();
                this._transaction?.Dispose();
            }

            this._transaction = null;
            this._connection = null;

            this._disposed = true;
        }

        protected abstract SQLiteConnection GetConnection();

        protected abstract void Close();

        /// <summary>
        /// トランザクションを開始します。
        /// </summary>
        /// <returns>トランザクションオブジェクト</returns>
        public ITransaction BeginTransaction()
        {
            this._lockObject.Enter();
            this._transaction = this.GetConnection().BeginTransaction();
            return new Transaction(this);
        }

        public void Commit()
        {
            if (this._transaction == null)
            {
                throw new InvalidOperationException("トランザクションが開始されていません。");
            }

            try
            {
                this._transaction.Commit();
            }
            finally
            {
                this._transaction.Dispose();
                this._transaction = null;
                this._lockObject.Exit();
            }
        }

        public void Roolback()
        {
            if (this._transaction == null)
            {
                throw new InvalidOperationException("トランザクションが開始されていません。");
            }

            try
            {
                this._transaction.Rollback();
            }
            finally
            {
                this._transaction.Dispose();
                this._transaction = null;
                this._lockObject.Exit();
            }
        }

        /// <summary>
        /// SQLを実行し、更新します。
        /// </summary>
        /// <param name="sql">データアクセサ</param>
        /// <returns>更新されたレコードが存在するならTrue。存在しなければFalseを返します。</returns>
        public bool Update(SqlBase sql)
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            if (this._transaction == null)
            {
                throw new InvalidOperationException("トランザクションが開始されていません。");
            }

            this._lockObject.Enter();
            try
            {
                using (var cmd = this.GetConnection().CreateCommand())
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
            finally
            {
                this._lockObject.Exit();
            }
        }

        /// <summary>
        /// SQLを実行し、リストを取得します。
        /// </summary>
        /// <typeparam name="T">戻り値のDto</typeparam>
        /// <param name="sql">データアクセサ</param>
        /// <returns>Dtoリスト</returns>
        public TDto[] ReadList<TDto>(SqlBase<TDto> sql)
            where TDto : IDto, new()
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            this._lockObject.Enter();
            try
            {
                using (var cmd = this.GetConnection().CreateCommand())
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
            finally
            {
                this._lockObject.Exit();
            }
        }

        /// <summary>
        /// SQLを実行し、1レコード取得します。
        /// </summary>
        /// <typeparam name="T">戻り値のDto型</typeparam>
        /// <param name="sql">データアクセサ</param>
        /// <returns>Dto</returns>
        public TDto? ReadLine<TDto>(SqlBase<TDto> sql)
            where TDto : class, IDto, new()
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            this._lockObject.Enter();
            try
            {
                using (var cmd = this.GetConnection().CreateCommand())
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
            finally
            {
                this._lockObject.Exit();
            }
        }

        /// <summary>
        /// SQLを実行し、値を取得します。
        /// </summary>
        /// <typeparam name="T">戻り値の型</typeparam>
        /// <param name="sql">データアクセサ</param>
        /// <returns>1オブジェクトの実行結果</returns>
        public T? ReadValue<T>(SqlBase sql)
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            this._lockObject.Enter();
            try
            {
                using (var cmd = this.GetConnection().CreateCommand())
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
            finally
            {
                this._lockObject.Exit();
            }
        }
    }
}
