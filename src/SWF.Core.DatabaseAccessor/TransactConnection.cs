using SWF.Core.Job;
using System.Data.Common;
using System.Data.SQLite;

namespace SWF.Core.DatabaseAccessor
{
    public sealed class TransactConnection
        : AbstractConnection, IConnection
    {
        private bool _disposed = false;
#pragma warning disable CA2213
        private SemaphoreSlim? _lockObject = null;
#pragma warning restore CA2213
        private DbTransaction? _transaction = null;
        private bool _isConnectionDispose;

        public async ValueTask Initialize(SemaphoreSlim lockObject, string filePath)
        {
            ArgumentNullException.ThrowIfNull(lockObject, nameof(lockObject));
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this._lockObject = lockObject;
            this.Connection = new SQLiteConnection($"Data Source={filePath}");
            await this.Connection.OpenAsync().False();
            this._transaction = await this.Connection.BeginTransactionAsync().False();
            this._isConnectionDispose = true;
        }

        public async ValueTask Initialize(SemaphoreSlim lockObject, SQLiteConnection connection)
        {
            ArgumentNullException.ThrowIfNull(lockObject, nameof(lockObject));
            ArgumentNullException.ThrowIfNull(connection, nameof(connection));

            this._lockObject = lockObject;
            this.Connection = connection;
            this._transaction = await this.Connection.BeginTransactionAsync().False();
            this._isConnectionDispose = false;
        }

        public async ValueTask DisposeAsync()
        {
            if (this._disposed)
            {
                return;
            }

            if (this._transaction is null)
            {
                throw new InvalidOperationException("トランザクションが設定されていません。");
            }

            if (this.IsOccursedException)
            {
                await this._transaction.RollbackAsync().False();
            }
            else
            {
                await this._transaction.CommitAsync().False();
            }

            await this._transaction.DisposeAsync().False();
            this._transaction = null;

            if (this._isConnectionDispose)
            {
                if (this.Connection is null)
                {
                    throw new InvalidOperationException("破棄するコネクションが設定されていません。");
                }

                await this.Connection.DisposeAsync().False();
            }

            this._lockObject?.Release();

            this._disposed = true;

            GC.SuppressFinalize(this);
        }
    }
}
