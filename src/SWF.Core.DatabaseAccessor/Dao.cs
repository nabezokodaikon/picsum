namespace SWF.Core.DatabaseAccessor
{
    /// <summary>
    /// DB管理
    /// </summary>
    public sealed partial class Dao<TConnection>
        : IDisposable
        where TConnection : AbstractConnection
    {
        public readonly static Dao<TConnection> Instance = new();

        private bool disposed = false;

        private Dao()
        {

        }

        ~Dao()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.connection?.Dispose();
                this.connection = null;
            }

            this.disposed = true;
        }

        // DBコネクション
        private TConnection? connection = null;

        /// <summary>
        /// DBに接続します。
        /// </summary>
        /// <param name="connection"></param>
        public void Connect(TConnection connection)
        {
            ArgumentNullException.ThrowIfNull(connection, nameof(connection));
            this.connection = connection;
        }

        /// <summary>
        /// トランザクションを開始します。
        /// </summary>
        /// <returns>トランザクションオブジェクト</returns>
        public Transaction BeginTransaction()
        {
            if (this.connection == null)
            {
                throw new NullReferenceException("コネクションがNullです。");
            }

            return this.connection.BeginTransaction();
        }

        /// <summary>
        /// SQLを実行し、更新します。
        /// </summary>
        /// <param name="sql">データアクセサ</param>
        /// <returns>更新されたレコードが存在するならTrue。存在しなければFalseを返します。</returns>
        public bool Update(SqlBase sql)
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            if (this.connection == null)
            {
                throw new NullReferenceException("コネクションがNullです。");
            }

            return this.connection.Update(sql);
        }

        /// <summary>
        /// SQLを実行し、リストを取得します。
        /// </summary>
        /// <typeparam name="TDto">戻り値のDto</typeparam>
        /// <param name="sql">データアクセサ</param>
        /// <returns>Dtoリスト</returns>
        public IList<TDto> ReadList<TDto>(SqlBase<TDto> sql) where TDto : IDto, new()
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            if (this.connection == null)
            {
                throw new NullReferenceException("コネクションがNullです。");
            }

            return this.connection.ReadList<TDto>(sql);
        }

        /// <summary>
        /// SQLを実行し、1レコード取得します。
        /// </summary>
        /// <typeparam name="TDto">戻り値のDto型</typeparam>
        /// <param name="sql">データアクセサ</param>
        /// <returns>Dto</returns>
        public TDto? ReadLine<TDto>(SqlBase<TDto> sql) where TDto : IDto, new()
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            if (this.connection == null)
            {
                throw new NullReferenceException("コネクションがNullです。");
            }

            return this.connection.ReadLine<TDto>(sql);
        }

        /// <summary>
        /// SQLを実行し、値を取得します。
        /// </summary>
        /// <typeparam name="TDto">戻り値の型</typeparam>
        /// <param name="sql">データアクセサ</param>
        /// <returns>1オブジェクトの実行結果</returns>
        public T? ReadValue<T>(SqlBase sql)
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            if (this.connection == null)
            {
                throw new NullReferenceException("コネクションがNullです。");
            }

            return this.connection.ReadValue<T>(sql);
        }
    }
}
