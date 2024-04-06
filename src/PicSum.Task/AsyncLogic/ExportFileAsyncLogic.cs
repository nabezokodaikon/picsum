using PicSum.Core.Task.AsyncTaskV2;
using System;
using System.IO;
using System.Runtime.Versioning;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// 画像ファイルエクスポート非同期ロジック
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class ExportFileAsyncLogic
        : AbstractAsyncLogic
    {
        public ExportFileAsyncLogic(IAsyncTask task)
            : base(task)
        {

        }

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
