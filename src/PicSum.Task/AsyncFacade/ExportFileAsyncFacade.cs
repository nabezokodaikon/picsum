using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Paramter;
using System;
using System.Threading;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// 画像ファイルエクスポート非同期ファサード
    /// </summary>
    public sealed class ExportFileAsyncFacade
        : OneWayFacadeBase<ExportFileParameter>
    {
        private static readonly ReaderWriterLockSlim facadeLock = new ReaderWriterLockSlim();

        /// <summary>
        /// 静的リソースを解放します。
        /// </summary>
        public static void DisposeStaticResouces()
        {
            ExportFileAsyncFacade.facadeLock.Dispose();
        }

        public override void Execute(ExportFileParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            ExportFileAsyncFacade.facadeLock.EnterWriteLock();

            try
            {
                var logic = new ExportFileAsyncLogic(this);
                logic.Execute(param.ExportDirectoryPath, param.FilePathList);
            }
            finally
            {
                ExportFileAsyncFacade.facadeLock.ExitWriteLock();
            }
        }
    }
}
