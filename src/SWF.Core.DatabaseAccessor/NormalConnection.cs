using SWF.Core.Job;
using System.Data.SQLite;

namespace SWF.Core.DatabaseAccessor
{
    public sealed class NormalConnection
        : AbstractConnection, IConnection
    {
        private bool _disposed = false;
#pragma warning disable CA2213
        private SemaphoreSlim? _lockObject = null;
#pragma warning restore CA2213
        private bool _isConnectionDispose;

        public async ValueTask Initialize(SemaphoreSlim lockObject, string filePath)
        {
            ArgumentNullException.ThrowIfNull(lockObject, nameof(lockObject));
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this.Connection = new SQLiteConnection($"Data Source={filePath}");
            await this.Connection.OpenAsync().False();

            this._lockObject = lockObject;
            this._isConnectionDispose = true;
        }

        public void Initialize(SemaphoreSlim lockObject, SQLiteConnection connection)
        {
            ArgumentNullException.ThrowIfNull(lockObject, nameof(lockObject));
            ArgumentNullException.ThrowIfNull(connection, nameof(connection));

            this.Connection = connection;
            this._lockObject = lockObject;
            this._isConnectionDispose = false;
        }

        public async ValueTask DisposeAsync()
        {
            if (this._disposed)
            {
                return;
            }

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
