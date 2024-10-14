using System.Data;
using System.Data.SQLite;

namespace SWF.Core.DatabaseAccessor
{
    /// <summary>
    /// DBに接続するベースクラスです。
    /// </summary>
    public abstract class ConnectionBase
        : IDisposable
    {
        private bool disposed = false;
        private readonly ReaderWriterLockSlim lockObject
            = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly SQLiteConnection connection;
        private SQLiteTransaction? transaction = null;

        public string DBFilePath { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dbFilePath">DBファイルパス</param>
        /// <param name="tableCreateSql">テーブル作成SQL</param>
        protected ConnectionBase(string dbFilePath, string tableCreateSql)
        {
            ArgumentException.ThrowIfNullOrEmpty(dbFilePath, nameof(dbFilePath));
            ArgumentException.ThrowIfNullOrEmpty(tableCreateSql, nameof(tableCreateSql));

            if (!File.Exists(dbFilePath))
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

            var connectionString = $"Data Source={dbFilePath}";
            this.connection = new SQLiteConnection("Data Source=:memory:");
            this.connection.Open();
            using (var fileConnection = new SQLiteConnection(connectionString))
            {
                fileConnection.Open();
                fileConnection.BackupDatabase(this.connection, "main", "main", -1, null, 0);
            }

            this.DBFilePath = dbFilePath;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                var connectionString = $"Data Source={this.DBFilePath}";
                using (var fileConnection = new SQLiteConnection(connectionString))
                {
                    fileConnection.Open();
                    this.connection.BackupDatabase(fileConnection, "main", "main", -1, null, 0);
                }

                this.connection.Close();
                this.transaction?.Dispose();
                this.lockObject.Dispose();
            }

            this.transaction = null;

            this.disposed = true;
        }

        /// <summary>
        /// リソースを解放します。
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ConnectionBase()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// トランザクションを開始します。
        /// </summary>
        /// <returns>トランザクションオブジェクト</returns>
        public Transaction BeginTransaction()
        {
            if (this.transaction != null)
            {
                throw new InvalidOperationException("トランザクションが実行中です。");
            }

            this.lockObject.EnterWriteLock();
            this.transaction = this.connection.BeginTransaction();
            return new Transaction(this);
        }

        public void Commit()
        {
            if (this.transaction == null)
            {
                throw new InvalidOperationException("トランザクションが開始されていません。");
            }

            try
            {
                this.transaction.Commit();
            }
            finally
            {
                this.transaction.Dispose();
                this.transaction = null;
                this.lockObject.ExitWriteLock();
            }
        }

        public void Roolback()
        {
            if (this.transaction == null)
            {
                throw new InvalidOperationException("トランザクションが開始されていません。");
            }

            try
            {
                this.transaction.Rollback();
            }
            finally
            {
                this.transaction.Dispose();
                this.transaction = null;
                this.lockObject.ExitWriteLock();
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

            if (this.transaction == null)
            {
                throw new InvalidOperationException("トランザクションが開始されていません。");
            }

            this.lockObject.EnterWriteLock();
            try
            {
                using (var cmd = this.connection.CreateCommand())
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
                this.lockObject.ExitWriteLock();
            }
        }

        /// <summary>
        /// SQLを実行し、リストを取得します。
        /// </summary>
        /// <typeparam name="T">戻り値のDto</typeparam>
        /// <param name="sql">データアクセサ</param>
        /// <returns>Dtoリスト</returns>
        public IList<TDto> ReadList<TDto>(SqlBase<TDto> sql)
            where TDto : IDto, new()
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            this.lockObject.EnterWriteLock();
            try
            {
                using (var cmd = this.connection.CreateCommand())
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

                            return list;
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
                this.lockObject.ExitWriteLock();
            }
        }

        /// <summary>
        /// SQLを実行し、1レコード取得します。
        /// </summary>
        /// <typeparam name="T">戻り値のDto型</typeparam>
        /// <param name="sql">データアクセサ</param>
        /// <returns>Dto</returns>
        public TDto? ReadLine<TDto>(SqlBase<TDto> sql) where TDto : IDto, new()
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            this.lockObject.EnterWriteLock();
            try
            {
                using (var cmd = this.connection.CreateCommand())
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
                            TDto dto = new TDto();
                            dto.Read(reader);
                            return dto;
                        }
                        else
                        {
                            return default;
                        }
                    }
                }
            }
            finally
            {
                this.lockObject.ExitWriteLock();
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

            this.lockObject.EnterWriteLock();
            try
            {
                using (var cmd = this.connection.CreateCommand())
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
                this.lockObject.ExitWriteLock();
            }
        }
    }
}
