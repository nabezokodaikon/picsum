using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Paramter;
using System;
using System.IO;
using System.Runtime.Versioning;
using System.Threading;

namespace PicSum.Task.AsyncTask
{
    /// <summary>
    /// 画像ファイルエクスポート非同期タスク
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class ExportFileAsyncTask
        : AbstractAsyncTask<ExportFileParameter>
    {
        private static readonly ReaderWriterLockSlim taskLock = new ReaderWriterLockSlim();

        /// <summary>
        /// 静的リソースを解放します。
        /// </summary>
        public static void DisposeStaticResouces()
        {
            ExportFileAsyncTask.taskLock.Dispose();
        }

        protected override void Execute(ExportFileParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            ExportFileAsyncTask.taskLock.EnterWriteLock();

            try
            {
                var logic = new ExportFileAsyncLogic(this);
                logic.Execute(param.SrcFilePath, param.ExportFilePath);
            }
            catch (PathTooLongException)
            {
                return;
            }
            catch (DirectoryNotFoundException)
            {
                return;
            }
            catch (FileNotFoundException)
            {
                return;
            }
            catch (IOException)
            {
                return;
            }
            catch (NotSupportedException)
            {
                return;
            }
            finally
            {
                ExportFileAsyncTask.taskLock.ExitWriteLock();
            }
        }
    }
}
