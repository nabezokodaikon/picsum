using System.Runtime.Versioning;

namespace SWF.Core.DatabaseAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class Transaction(AbstractConnection connection)
        : ITransaction
    {
        private bool _disposed = false;

        // コネクション
        private readonly AbstractConnection _conntenction
            = connection ?? throw new ArgumentNullException(nameof(connection));

        // コミットフラグ
        private bool _isCommitted = false;

        /// <summary>
        /// コミットします。
        /// </summary>
        public void Commit()
        {
            this._conntenction.Commit();
            this._isCommitted = true;
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                if (!this._isCommitted)
                {
                    this._conntenction.Roolback();
                }
            }

            this._disposed = true;
        }

        /// <summary>
        /// リソースを解放します。
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
