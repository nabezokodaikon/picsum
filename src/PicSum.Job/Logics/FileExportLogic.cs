using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// 画像ファイルエクスポート非同期ロジック
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class FileExportLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        internal static readonly ReaderWriterLockSlim FileExportLock = new();

        /// <summary>
        /// 静的リソースを解放します。
        /// </summary>
        public static void DisposeStaticResouces()
        {
            FileExportLock.Dispose();
        }

        public void Execute(string srcFilePath, string exportFilePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(srcFilePath, nameof(srcFilePath));
            ArgumentException.ThrowIfNullOrEmpty(exportFilePath, nameof(exportFilePath));

            File.Copy(srcFilePath, exportFilePath, false);
        }
    }
}
