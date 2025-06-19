using SWF.Core.ConsoleAccessor;
using SWF.Core.FileAccessor;
using System.Data.SQLite;
using System.Runtime.Versioning;

namespace SWF.Core.DatabaseAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public abstract class AbstractDB
        : IDB
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
                con.Close();
            }
        }

        private static SQLiteConnection CreateFileConnection(string filePath)
        {
            var connection = new SQLiteConnection($"Data Source={filePath}");
            connection.Open();
            return connection;
        }

        private static SQLiteConnection CreateMemoryConnection(string filePath)
        {
            var connection = new SQLiteConnection("Data Source=:memory:");
            connection.Open();
            using (var fileConnection = new SQLiteConnection($"Data Source={filePath}"))
            {
                fileConnection.Open();
                fileConnection.BackupDatabase(connection, "main", "main", -1, null, 0);
                fileConnection.Close();
            }

            return connection;
        }

        private bool _disposed = false;
        private readonly string _filePath;
        private readonly bool _isPersistence;
        private SQLiteConnection? _connection;
        private readonly Lock _lockObject = new();

        protected AbstractDB(string filePath, string tablesCreateSql, bool isPersistence)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentNullException.ThrowIfNullOrEmpty(tablesCreateSql, nameof(tablesCreateSql));

            if (!FileUtil.IsExistsFile(filePath))
            {
                CreateDB(filePath, tablesCreateSql);
            }

            this._filePath = filePath;
            this._isPersistence = isPersistence;
        }

        public IDBConnection Connect()
        {
            this._lockObject.Enter();

            Log.GetLogger().Trace("DBへの接続ロックを開始します。");

            if (this._isPersistence)
            {
                this._connection ??= CreateMemoryConnection(this._filePath);
                return new DBConnection(
                    this._lockObject, this._connection, false);
            }
            else
            {
                return new DBConnection(
                    this._lockObject, CreateFileConnection(this._filePath), false);
            }
        }

        public IDBConnection ConnectWithTransaction()
        {
            this._lockObject.Enter();

            Log.GetLogger().Trace("DBへのトランザクション接続ロックを開始します。");

            if (this._isPersistence)
            {
                this._connection ??= CreateMemoryConnection(this._filePath);
                return new DBConnection(
                    this._lockObject, this._connection, true);
            }
            else
            {
                return new DBConnection(
                    this._lockObject, CreateFileConnection(this._filePath), true);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                if (this._connection != null)
                {
                    using (var fileConnection = new SQLiteConnection($"Data Source={this._filePath}"))
                    {
                        fileConnection.Open();
                        this._connection.BackupDatabase(fileConnection, "main", "main", -1, null, 0);
                    }

                    this._connection?.Close();
                    this._connection?.Dispose();
                }
            }

            this._connection = null;

            this._disposed = true;
        }
    }
}
