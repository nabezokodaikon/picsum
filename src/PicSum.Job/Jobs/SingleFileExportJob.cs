using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// 画像ファイルエクスポート非同期ジョブ
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class SingleFileExportJob
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

            Instance<IFileExporter>.Value.Lock.Wait();

            try
            {
                Instance<IFileExporter>.Value.Execute(param.SrcFilePath, param.ExportFilePath);
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
                Instance<IFileExporter>.Value.Lock.Release();
            }
        }
    }
}
