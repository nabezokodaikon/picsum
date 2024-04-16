using PicSum.Core.Job.AsyncJob;
using System;
using System.IO;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// 画像ファイルエクスポート非同期ロジック
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class FileExportLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public void Execute(string srcFilePath, string exportFilePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(srcFilePath, nameof(srcFilePath));
            ArgumentException.ThrowIfNullOrEmpty(exportFilePath, nameof(exportFilePath));

            File.Copy(srcFilePath, exportFilePath);
        }
    }
}
