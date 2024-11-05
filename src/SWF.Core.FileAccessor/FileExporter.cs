using System.Runtime.Versioning;

namespace SWF.Core.FileAccessor
{
    /// <summary>
    /// 画像ファイルエクスポート非同期ロジック
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed partial class FileExporter
        : IFileExporter
    {
        private bool disposed = false;
        public SemaphoreSlim Lock { get; private set; }

        public FileExporter()
        {
            this.Lock = new(1, 1);
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
