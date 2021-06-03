using System;

namespace PicSum.Core.Data.DatabaseAccessor
{
    /// <summary>
    /// DBトランザクション
    /// </summary>
    public class Transaction : IDisposable
    {
        // コネクション
        private ConnectionBase _conntenction = null;

        // コミットフラグ
        private bool _isCommitted = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Transaction(ConnectionBase connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("conntenction");
            }

            _conntenction = connection;
        }

        /// <summary>
        /// コミットします。
        /// </summary>
        public void Commit()
        {
            _conntenction.Commit();
            _isCommitted = true;
        }

        /// <summary>
        /// リソースを解放します。
        /// </summary>
        public void Dispose()
        {
            if (!_isCommitted)
            {
                _conntenction.Roolback();
            }

            _conntenction = null;
        }
    }
}
