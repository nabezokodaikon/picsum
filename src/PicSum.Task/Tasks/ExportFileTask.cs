using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Logics;
using PicSum.Task.Paramters;
using System;
using System.IO;
using System.Runtime.Versioning;
using System.Threading;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// 画像ファイルエクスポート非同期タスク
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class ExportFileTask
        : AbstractOneWayTask<ExportFileParameter>
    {
        private static readonly ReaderWriterLockSlim taskLock = new ReaderWriterLockSlim();

        /// <summary>
        /// 静的リソースを解放します。
        /// </summary>
        public static void DisposeStaticResouces()
        {
            ExportFileTask.taskLock.Dispose();
        }

        protected override void Execute(ExportFileParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            ExportFileTask.taskLock.EnterWriteLock();

            try
            {
                var logic = new ExportFileLogic(this);
                logic.Execute(param.SrcFilePath, param.ExportFilePath);
            }
            catch (PathTooLongException ex)
            {
                this.WriteErrorLog(ex);
                return;
            }
            catch (DirectoryNotFoundException ex)
            {
                this.WriteErrorLog(ex);
                return;
            }
            catch (FileNotFoundException ex)
            {
                this.WriteErrorLog(ex);
                return;
            }
            catch (IOException ex)
            {
                this.WriteErrorLog(ex);
                return;
            }
            catch (NotSupportedException ex)
            {
                this.WriteErrorLog(ex);
                return;
            }
            finally
            {
                ExportFileTask.taskLock.ExitWriteLock();
            }
        }
    }
}
