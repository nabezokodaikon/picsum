using System;
using System.Threading;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// 画像ファイルエクスポート非同期ファサード
    /// </summary>
    public class ExportFileAsyncFacade
        : OneWayFacadeBase<ExportFileParameter>
    {
        private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        /// <summary>
        /// 静的リソースを解放します。
        /// </summary>
        public static void DisposeStaticResouces()
        {
            _lock.Dispose();
        }

        public override void Execute(ExportFileParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            _lock.EnterWriteLock();

            try
            {
                ExportFileAsyncLogic logic = new ExportFileAsyncLogic(this);
                logic.Execute(param.ExportDirectoryPath, param.FilePathList);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}
