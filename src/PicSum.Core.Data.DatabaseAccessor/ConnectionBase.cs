using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using PicSum.Core.Base.Exception;

namespace PicSum.Core.Data.DatabaseAccessor
{
    /// <summary>
    /// DBに接続するベースクラスです。
    /// </summary>
    public abstract class ConnectionBase : IDisposable
    {
        #region インスタンス変数

        private readonly ReaderWriterLockSlim _transactionLock = new ReaderWriterLockSlim();
        private readonly ReaderWriterLockSlim _executeSqlLock = new ReaderWriterLockSlim();
        private readonly SQLiteConnection _connection = null;
        private SQLiteTransaction _transaction = null;

        #endregion

        #region プライベートプロパティ

        private SQLiteConnection Connection
        {
            get
            {
                return _connection;
            }
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dbFilePath">DBファイルパス</param>
        /// <param name="tableCreateSql">テーブル作成SQL</param>
        protected ConnectionBase(string dbFilePath, string tableCreateSql)
        {
            if (dbFilePath == null)
            {
                throw new ArgumentNullException("dbFilePath");
            }

            if (!File.Exists(dbFilePath))
            {
                SQLiteConnection.CreateFile(dbFilePath);
                using (var con = new SQLiteConnection(string.Format("Data Source={0}", dbFilePath)))
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

            string connectionString = string.Format("Data Source={0}", dbFilePath);
            _connection = new SQLiteConnection(connectionString);
            _connection.Open();
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// リソースを解放します。
        /// </summary>
        public void Dispose()
        {
            _connection.Close();
            _transactionLock.Dispose();
            _executeSqlLock.Dispose();
        }

        /// <summary>
        /// トランザクションを開始します。
        /// </summary>
        /// <returns>トランザクションオブジェクト</returns>
        public Transaction BeginTransaction()
        {
            _transactionLock.EnterWriteLock();
            _executeSqlLock.EnterWriteLock();

            try
            {
                if (_transaction != null)
                {
                    throw new Exception("トランザクションが実行中です。");
                }

                _transaction = Connection.BeginTransaction();
                return new Transaction(this);
            }
            catch (Exception)
            {
                _transactionLock.ExitWriteLock();
                throw;
            }
            finally
            {
                _executeSqlLock.ExitWriteLock();
            }
        }

        public void Commit()
        {
            if (_transaction == null)
            {
                throw new NullReferenceException("_transaction");
            }

            _executeSqlLock.EnterWriteLock();

            try
            {
                _transaction.Commit();
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
                _executeSqlLock.ExitWriteLock();
                _transactionLock.ExitWriteLock();
            }
        }

        public void Roolback()
        {
            if (_transaction == null)
            {
                throw new NullReferenceException("_transaction");
            }

            _executeSqlLock.EnterWriteLock();

            try
            {
                _transaction.Commit();
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
                _executeSqlLock.ExitWriteLock();
                _transactionLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// SQLを実行し、更新します。
        /// </summary>
        /// <param name="sql">データアクセサ</param>
        /// <returns>更新されたレコードが存在するならTrue。存在しなければFalseを返します。</returns>
        public bool Update(SqlBase sql)
        {
            if (sql == null)
            {
                throw new ArgumentNullException("sql");
            }

            _executeSqlLock.EnterWriteLock();

            try
            {
                using (var cmd = Connection.CreateCommand())
                {
                    cmd.CommandText = sql.GetExecuteSql();

                    if (sql.ParameterList.Count > 0)
                    {
                        cmd.Parameters.AddRange(sql.ParameterList.ToArray());
                    }

                    int result = ExecuteNonQuery(sql.GetType().Name, cmd);
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
                _executeSqlLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// SQLを実行し、リストを取得します。
        /// </summary>
        /// <typeparam name="T">戻り値のDto</typeparam>
        /// <param name="sql">データアクセサ</param>
        /// <returns>Dtoリスト</returns>
        public IList<TDto> ReadList<TDto>(SqlBase<TDto> sql) where TDto : IDto, new()
        {
            if (sql == null)
            {
                throw new ArgumentNullException("sql");
            }

            _executeSqlLock.EnterReadLock();

            try
            {
                using (var cmd = Connection.CreateCommand())
                {
                    cmd.CommandText = sql.GetExecuteSql();

                    if (sql.ParameterList.Count > 0)
                    {
                        cmd.Parameters.AddRange(sql.ParameterList.ToArray());
                    }

                    using (var reader = ExecuteReader(sql.GetType().Name, cmd, CommandBehavior.Default))
                    {
                        if (reader.HasRows)
                        {
                            IList<TDto> list = new List<TDto>();

                            while (reader.Read())
                            {
                                TDto dto = new TDto();
                                dto.Read(reader);
                                list.Add(dto);
                            }

                            return list;
                        }
                        else
                        {
                            return new List<TDto>();
                        }
                    }
                }
            }
            finally
            {
                _executeSqlLock.ExitReadLock();
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
            if (sql == null)
            {
                throw new ArgumentNullException("sql");
            }

            _executeSqlLock.EnterReadLock();

            try
            {
                using (SQLiteCommand cmd = Connection.CreateCommand())
                {
                    cmd.CommandText = sql.GetExecuteSql();

                    if (sql.ParameterList.Count > 0)
                    {
                        cmd.Parameters.AddRange(sql.ParameterList.ToArray());
                    }

                    using (SQLiteDataReader reader = ExecuteReader(sql.GetType().Name, cmd, CommandBehavior.SingleRow))
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
                _executeSqlLock.ExitReadLock();
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
            if (sql == null)
            {
                throw new ArgumentNullException("sql");
            }

            _executeSqlLock.EnterReadLock();

            try
            {
                using (var cmd = Connection.CreateCommand())
                {
                    cmd.CommandText = sql.GetExecuteSql();

                    if (sql.ParameterList.Count > 0)
                    {
                        cmd.Parameters.AddRange(sql.ParameterList.ToArray());
                    }

                    object result = ExecuteScalar(sql.GetType().Name, cmd);
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
                _executeSqlLock.ExitReadLock();
            }
        }

        #endregion

        #region プライベートメソッド

        private int ExecuteNonQuery(string sqlTitle, SQLiteCommand cmd)
        {
            return cmd.ExecuteNonQuery();
        }

        private SQLiteDataReader ExecuteReader(string sqlTitle, SQLiteCommand cmd, CommandBehavior cb)
        {
            return cmd.ExecuteReader(cb);
        }

        private object ExecuteScalar(string sqlTitle, SQLiteCommand cmd)
        {
            return cmd.ExecuteScalar();
        }

        #endregion
    }
}
