using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// 画像ファイルエクスポート非同期ジョブ
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class SingleFileExportJob
        : AbstractOneWayJob<SingleFileExportParameter>
    {
        protected override void Execute(SingleFileExportParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            if (param.SrcFilePath == null)
            {
                throw new ArgumentException("エクスポート元のファイルパスがNULLです。", nameof(param));
            }

            if (param.ExportFilePath == null)
            {
                throw new ArgumentException("エクスポート先のファイルパスがNULLです。", nameof(param));
            }

            FileExportLogic.FileExportLock.EnterWriteLock();

            try
            {
                var logic = new FileExportLogic(this);
                logic.Execute(param.SrcFilePath, param.ExportFilePath);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new JobException(this.ID, ex);
            }
            catch (PathTooLongException ex)
            {
                throw new JobException(this.ID, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new JobException(this.ID, ex);
            }
            catch (FileNotFoundException ex)
            {
                throw new JobException(this.ID, ex);
            }
            catch (IOException ex)
            {
                throw new JobException(this.ID, ex);
            }
            catch (NotSupportedException ex)
            {
                throw new JobException(this.ID, ex);
            }
            finally
            {
                FileExportLogic.FileExportLock.ExitWriteLock();
            }
        }
    }
}
