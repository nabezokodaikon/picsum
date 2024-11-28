using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class MultiFilesConvertJob
        : AbstractTwoWayJob<MultiFilesConvertParameter, ValueResult<string>>
    {
        protected override void Execute(MultiFilesConvertParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            if (param.SrcFiles == null)
            {
                throw new ArgumentException("変換元のファイルパスリストがNULLです。", nameof(param));
            }

            if (param.ExportDirecotry == null)
            {
                throw new ArgumentException("変換先のファイルパスがNULLです。", nameof(param));
            }

            Instance<IFileConverter>.Value.Lock.Enter();

            try
            {
                foreach (var srcFile in param.SrcFiles)
                {
                    this.CheckCancel();

                    try
                    {
                        var exportFilePath = Path.Combine(param.ExportDirecotry,
                            $"{FileUtil.GetFileNameWithoutExtension(srcFile)}{FileUtil.GetImageFileExtension(param.ImageFileFormat).ToLower()}");
                        var renameFileName = FileUtil.GetExportFileName(
                            param.ExportDirecotry, exportFilePath);
                        var renameFilePath = Path.Combine(
                            param.ExportDirecotry, renameFileName);
                        Instance<IFileConverter>.Value.Execute(
                            srcFile, renameFilePath, param.ImageFileFormat);
                        var result = new ValueResult<string>
                        {
                            Value = renameFileName,
                        };
                        this.Callback(result);
                    }
                    catch (FileUtilException ex)
                    {
                        this.WriteErrorLog(new JobException(this.ID, ex));
                    }
                }
            }
            finally
            {
                Instance<IFileConverter>.Value.Lock.Exit();
            }
        }
    }
}
