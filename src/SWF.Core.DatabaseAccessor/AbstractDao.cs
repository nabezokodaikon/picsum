using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Data.SQLite;

namespace SWF.Core.DatabaseAccessor
{

    public abstract class AbstractDao
        : IDisposable
    {
        private static void CreateDB(string filePath, string tablesCreateSql)
        {
            SQLiteConnection.CreateFile(filePath);
            using (var con = new SQLiteConnection($"Data Source={filePath}"))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
#pragma warning disable CA2100
                    cmd.CommandText = tablesCreateSql;
#pragma warning restore CA2100
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

        protected AbstractDao(string filePath, string tablesCreateSql, bool isPersistent)
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

        public async ValueTask<IConnection> Connect()
        {
            await this._lockObject.WaitAsync().False();

            if (this._isPersistent)
            {
                this._persistentConnection ??= CreateInMemoryConnection(this._filePath);
                var con = new NormalConnection();
                con.Initialize(this._lockObject, this._persistentConnection);
                return con;
            }
            else
            {
                var con = new NormalConnection();
                await con.Initialize(this._lockObject, this._filePath).False();
                return con;
            }
        }

        public async ValueTask<IConnection> ConnectWithTransaction()
        {
            await this._lockObject.WaitAsync().False();

            if (this._isPersistent)
            {
                this._persistentConnection ??= CreateInMemoryConnection(this._filePath);
                var con = new TransactConnection();
                await con.Initialize(this._lockObject, this._persistentConnection).False();
                return con;
            }
            else
            {
                var con = new TransactConnection();
                await con.Initialize(this._lockObject, this._filePath).False();
                return con;
            }
        }

        public void Dispose()
        {
            if (this._disposed)
            {
                return;
            }

            if (this._persistentConnection is not null)
            {
                using (var fileConnection = new SQLiteConnection($"Data Source={this._filePath}"))
                {
                    fileConnection.Open();
                    this._persistentConnection.BackupDatabase(fileConnection, "main", "main", -1, null, 0);
                }

                this._persistentConnection.Dispose();
                this._lockObject.Dispose();

                this._persistentConnection = null;
            }

            this._disposed = true;

            GC.SuppressFinalize(this);
        }
    }
}