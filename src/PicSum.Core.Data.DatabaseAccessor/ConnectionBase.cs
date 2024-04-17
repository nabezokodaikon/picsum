using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Threading;

namespace PicSum.Core.Data.DatabaseAccessor
{
    /// <summary>
    /// DBに接続するベースクラスです。
    /// </summary>
    public abstract class ConnectionBase
        : IDisposable
    {
        private bool disposed = false;
        private readonly ReaderWriterLockSlim transactionLock = new();
        private readonly ReaderWriterLockSlim executeSqlLock = new();
        private readonly SQLiteConnection connection;
        private SQLiteTransaction transaction = null;

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
            this.connection = new SQLiteConnection(connectionString);
            this.connection.Open();

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
                this.connection.Close();
                this.transaction?.Dispose();
                this.transactionLock.Dispose();
                this.executeSqlLock.Dispose();
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
            this.transactionLock.EnterWriteLock();
            this.executeSqlLock.EnterWriteLock();

            if (this.transaction != null)
            {
                throw new InvalidOperationException("トランザクションが実行中です。");
            }

            try
            {
                this.transaction = this.connection.BeginTransaction();
                return new Transaction(this);
            }
            finally
            {
                this.executeSqlLock.ExitWriteLock();
            }
        }

        public void Commit()
        {
            this.executeSqlLock.EnterWriteLock();

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
                this.executeSqlLock.ExitWriteLock();
                this.transactionLock.ExitWriteLock();
            }
        }

        public void Roolback()
        {
            this.executeSqlLock.EnterWriteLock();

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
                this.executeSqlLock.ExitWriteLock();
                this.transactionLock.ExitWriteLock();
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

            this.executeSqlLock.EnterWriteLock();

            if (this.transaction == null)
            {
                throw new InvalidOperationException("トランザクションが開始されていません。");
            }

            try
            {
                using (var cmd = this.connection.CreateCommand())
                {
                    cmd.CommandText = sql.GetExecuteSql();

                    if (sql.ParameterList.Count > 0)
                    {
                        cmd.Parameters.AddRange(sql.ParameterList.ToArray());
                    }

                    var result = this.ExecuteNonQuery(cmd);
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
                this.executeSqlLock.ExitWriteLock();
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

            this.executeSqlLock.EnterReadLock();

            try
            {
                using (var cmd = this.connection.CreateCommand())
                {
                    cmd.CommandText = sql.GetExecuteSql();

                    if (sql.ParameterList.Count > 0)
                    {
                        cmd.Parameters.AddRange(sql.ParameterList.ToArray());
                    }

                    using (var reader = this.ExecuteReader(cmd, CommandBehavior.Default))
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
                this.executeSqlLock.ExitReadLock();
            }
        }

        /// <summary>
        /// SQLを実行し、1レコード取得します。
        /// </summary>
        /// <typeparam name="T">戻り値のDto型</typeparam>
        /// <param name="sql">データアクセサ</param>
        /// <returns>Dto</returns>
        public TDto ReadLine<TDto>(SqlBase<TDto> sql) where TDto : IDto, new()
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            this.executeSqlLock.EnterReadLock();

            try
            {
                using (var cmd = this.connection.CreateCommand())
                {
                    cmd.CommandText = sql.GetExecuteSql();

                    if (sql.ParameterList.Count > 0)
                    {
                        cmd.Parameters.AddRange(sql.ParameterList.ToArray());
                    }

                    using (var reader = this.ExecuteReader(cmd, CommandBehavior.SingleRow))
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
                this.executeSqlLock.ExitReadLock();
            }
        }

        /// <summary>
        /// SQLを実行し、値を取得します。
        /// </summary>
        /// <typeparam name="T">戻り値の型</typeparam>
        /// <param name="sql">データアクセサ</param>
        /// <returns>1オブジェクトの実行結果</returns>
        public T ReadValue<T>(SqlBase sql)
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            this.executeSqlLock.EnterReadLock();

            try
            {
                using (var cmd = this.connection.CreateCommand())
                {
                    cmd.CommandText = sql.GetExecuteSql();

                    if (sql.ParameterList.Count > 0)
                    {
                        cmd.Parameters.AddRange(sql.ParameterList.ToArray());
                    }

                    var result = this.ExecuteScalar(cmd);
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
                this.executeSqlLock.ExitReadLock();
            }
        }

        private int ExecuteNonQuery(SQLiteCommand cmd)
        {
            return cmd.ExecuteNonQuery();
        }

        private SQLiteDataReader ExecuteReader(SQLiteCommand cmd, CommandBehavior cb)
        {
            return cmd.ExecuteReader(cb);
        }

        private object ExecuteScalar(SQLiteCommand cmd)
        {
            return cmd.ExecuteScalar();
        }
    }
}
