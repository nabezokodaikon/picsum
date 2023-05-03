using System;

namespace PicSum.Core.Data.DatabaseAccessor
{
    /// <summary>
    /// DBトランザクション
    /// </summary>
    public sealed class Transaction
        : IDisposable
    {
        private bool disposed = false;

        // コネクション
        private ConnectionBase conntenction = null;

        // コミットフラグ
        private bool isCommitted = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Transaction(ConnectionBase connection)
        {
            this.conntenction = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <summary>
        /// コミットします。
        /// </summary>
        public void Commit()
        {
            this.conntenction.Commit();
            this.isCommitted = true;
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                if (!isCommitted)
                {
                    this.conntenction.Roolback();
                }
            }

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

        ~Transaction()
        {
            this.Dispose(false);
        }
    }
}