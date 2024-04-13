using PicSum.Core.Task.AsyncTaskV2;
using System;
using System.IO;
using System.Runtime.Versioning;

namespace PicSum.Task.Logics
{
    /// <summary>
    /// 画像ファイルエクスポート非同期ロジック
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class FileExportLogic(IAsyncTask task)
        : AbstractAsyncLogic(task)
    {
        public void Execute(string srcFilePath, string exportFilePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(srcFilePath, nameof(srcFilePath));
            ArgumentException.ThrowIfNullOrEmpty(exportFilePath, nameof(exportFilePath));

            File.Copy(srcFilePath, exportFilePath);
        }
    }
}
