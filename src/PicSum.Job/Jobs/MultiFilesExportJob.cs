using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using SWF.Core.FileAccessor;

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

            var logic = new FileExportLogic(this);

            FileExportLogic.FileExportLock.EnterWriteLock();

            try
            {
                foreach (var srcFile in param.SrcFiles)
                {
                    this.CheckCancel();

                    try
                    {
                        var exportFileName = FileUtil.GetExportFileName(param.ExportDirecotry, srcFile);
                        var exportFilePath = Path.Combine(param.ExportDirecotry, exportFileName);
                        logic.Execute(srcFile, exportFilePath);
                        var result = new ValueResult<string>
                        {
                            Value = exportFileName,
                        };
                        this.Callback(result);
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
                FileExportLogic.FileExportLock.ExitWriteLock();
            }
        }
    }
}
