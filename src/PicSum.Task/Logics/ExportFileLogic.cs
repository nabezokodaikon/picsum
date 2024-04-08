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
    internal sealed class ExportFileLogic(IAsyncTask task)
        : AbstractAsyncLogic(task)
    {
        public void Execute(string srcFilePath, string exportFilePath)
        {
            if (srcFilePath == null)
            {
                throw new ArgumentNullException(nameof(srcFilePath));
            }

            if (exportFilePath == null)
            {
                throw new ArgumentNullException(nameof(exportFilePath));
            }

            File.Copy(srcFilePath, exportFilePath);
        }
    }
}
