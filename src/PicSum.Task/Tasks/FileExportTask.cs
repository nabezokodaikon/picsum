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
                throw new TaskException(this.ID, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new TaskException(this.ID, ex);
            }
            catch (FileNotFoundException ex)
            {
                throw new TaskException(this.ID, ex);
            }
            catch (IOException ex)
            {
                throw new TaskException(this.ID, ex);
            }
            catch (NotSupportedException ex)
            {
                throw new TaskException(this.ID, ex);
            }
            finally
            {
                FileExportTask.taskLock.ExitWriteLock();
            }
        }
    }
}
