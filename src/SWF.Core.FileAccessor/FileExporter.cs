using System.Runtime.Versioning;

namespace SWF.Core.FileAccessor
{
    /// <summary>
    /// 画像ファイルエクスポート非同期ロジック
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed partial class FileExporter
        : IDisposable
    {
        public readonly static FileExporter Instance = new();

        private bool disposed = false;
        public readonly SemaphoreSlim Lock = new(1, 1);

        private FileExporter()
        {

        }

        ~FileExporter()
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
                this.Lock.Dispose();
            }

            this.disposed = true;
        }

        public void Execute(string srcFilePath, string exportFilePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(srcFilePath, nameof(srcFilePath));
            ArgumentException.ThrowIfNullOrEmpty(exportFilePath, nameof(exportFilePath));

            File.Copy(srcFilePath, exportFilePath, false);
        }
    }
}
