using SWF.Core.FileAccessor;
using System.Data.SQLite;
using System.Runtime.Versioning;

namespace SWF.Core.DatabaseAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public abstract class AbstractDatabase
        : IAsyncDisposable
    {
        private static void CreateDB(string filePath, string tablesCreateSql)
        {
            SQLiteConnection.CreateFile(filePath);
            using (var con = new SQLiteConnection($"Data Source={filePath}"))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = tablesCreateSql;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static SQLiteConnection CreateInMemoryConnection(string filePath)
        {
            var memoryConnection = new SQLiteConnection("Data Source=:memory:");
            memoryConnection.Open();
            using (var fileConnection = new SQLiteConnection($"Data Source={filePath}"))
            {
                fileConnection.Open();
                fileConnection.BackupDatabase(memoryConnection, "main", "main", -1, null, 0);
            }

            return memoryConnection;
        }

        private bool _disposed = false;
        private readonly string _filePath;
        private readonly bool _isPersistent;
        private SQLiteConnection? _persistentConnection;
        private readonly SemaphoreSlim _lockObject = new(1, 1);

        protected AbstractDatabase(string filePath, string tablesCreateSql, bool isPersistent)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentNullException.ThrowIfNullOrEmpty(tablesCreateSql, nameof(tablesCreateSql));

            if (!FileUtil.IsExistsFile(filePath))
            {
                CreateDB(filePath, tablesCreateSql);
            }

            this._filePath = filePath;
            this._isPersistent = isPersistent;
        }

        public async Task<IDatabaseConnection> Connect()
        {
            await this._lockObject.WaitAsync();

            if (this._isPersistent)
            {
                this._persistentConnection ??= CreateInMemoryConnection(this._filePath);
                return new DatabaseConnection(
                    this._lockObject, this._persistentConnection, false);
            }
            else
            {
                return new DatabaseConnection(
                    this._lockObject, this._filePath, false);
            }
        }

        public async Task<IDatabaseConnection> ConnectWithTransaction()
        {
            await this._lockObject.WaitAsync();

            if (this._isPersistent)
            {
                this._persistentConnection ??= CreateInMemoryConnection(this._filePath);
                return new DatabaseConnection(
                    this._lockObject, this._persistentConnection, true);
            }
            else
            {
                return new DatabaseConnection(
                    this._lockObject, this._filePath, true);
            }
        }

        public ValueTask DisposeAsync()
        {
            if (this._disposed)
            {
                return ValueTask.CompletedTask;
            }

            if (this._persistentConnection != null)
            {
                using (var fileConnection = new SQLiteConnection($"Data Source={this._filePath}"))
                {
                    fileConnection.Open();
                    this._persistentConnection.BackupDatabase(fileConnection, "main", "main", -1, null, 0);
                }

                this._persistentConnection?.Dispose();
            }

            this._persistentConnection = null;
            this._lockObject.Dispose();

            this._disposed = true;

            GC.SuppressFinalize(this);

            return ValueTask.CompletedTask;
        }
    }
}
