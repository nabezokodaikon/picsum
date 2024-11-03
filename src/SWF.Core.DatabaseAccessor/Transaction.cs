namespace SWF.Core.DatabaseAccessor
{
    /// <summary>
    /// DBトランザクション
    /// </summary>
    /// <remarks>
    /// コンストラクタ
    /// </remarks>
    public sealed partial class Transaction(AbstractConnection connection)
        : IDisposable
    {
        private bool disposed = false;

        // コネクション
        private readonly AbstractConnection conntenction
            = connection ?? throw new ArgumentNullException(nameof(connection));

        // コミットフラグ
        private bool isCommitted = false;

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
                if (!this.isCommitted)
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
