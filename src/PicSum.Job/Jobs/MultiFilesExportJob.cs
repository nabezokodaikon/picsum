using PicSum.Job.Parameters;
using SWF.Core.FileAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{
    public sealed class MultiFilesExportJob
        : AbstractTwoWayJob<MultiFilesExportParameter, ValueResult<string>>
    {
        protected override void Execute(MultiFilesExportParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            if (param.SrcFiles == null)
            {
                throw new ArgumentException("エクスポート元のファイルパスリストがNULLです。", nameof(param));
            }

            if (param.ExportDirecotry == null)
            {
                throw new ArgumentException("エクスポート先のファイルパスがNULLです。", nameof(param));
            }

            FileExporter.Instance.Lock.Wait();

            try
            {
                foreach (var srcFile in param.SrcFiles)
                {
                    this.CheckCancel();

                    try
                    {
                        var exportFileName = FileUtil.GetExportFileName(param.ExportDirecotry, srcFile);
                        var exportFilePath = Path.Combine(param.ExportDirecotry, exportFileName);
                        FileExporter.Instance.Execute(srcFile, exportFilePath);
                        var result = new ValueResult<string>
                        {
                            Value = exportFileName,
                        };
                        this.Callback(result);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        throw new JobException(this.ID, ex);
                    }
                    catch (PathTooLongException ex)
                    {
                        this.WriteErrorLog(new JobException(this.ID, ex));
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        this.WriteErrorLog(new JobException(this.ID, ex));
                    }
                    catch (FileNotFoundException ex)
                    {
                        this.WriteErrorLog(new JobException(this.ID, ex));
                    }
                    catch (IOException ex)
                    {
                        this.WriteErrorLog(new JobException(this.ID, ex));
                    }
                    catch (NotSupportedException ex)
                    {
                        this.WriteErrorLog(new JobException(this.ID, ex));
                    }
                }
            }
            finally
            {
                FileExporter.Instance.Lock.Release();
            }
        }
    }
}
