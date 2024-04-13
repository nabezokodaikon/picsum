using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Logics;
using PicSum.Task.Paramters;
using System;
using System.IO;
using System.Runtime.Versioning;
using System.Threading;
using static System.Windows.Forms.AxHost;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// 画像ファイルエクスポート非同期タスク
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class FileExportTask
        : AbstractOneWayTask<ExportFileParameter>
    {
        private static readonly ReaderWriterLockSlim taskLock = new();

        /// <summary>
        /// 静的リソースを解放します。
        /// </summary>
        public static void DisposeStaticResouces()
        {
            FileExportTask.taskLock.Dispose();
        }

        protected override void Execute(ExportFileParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            FileExportTask.taskLock.EnterWriteLock();

            try
            {
                var logic = new FileExportLogic(this);
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
                FileExportTask.taskLock.ExitWriteLock();
            }
        }
    }
}
