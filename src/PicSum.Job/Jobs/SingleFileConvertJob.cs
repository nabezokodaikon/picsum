using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{
    internal sealed class SingleFileConvertJob
        : AbstractOneWayJob<SingleFileConvertParameter>
    {
        protected override void Execute(SingleFileConvertParameter parameter)
        {
            if (parameter.SrcFilePath == null)
            {
                throw new ArgumentException("変換元のファイルパスがNULLです。", nameof(parameter));
            }

            if (parameter.ExportFilePath == null)
            {
                throw new ArgumentException("変換先先のファイルパスがNULLです。", nameof(parameter));
            }

            Instance<IFileConverter>.Value.Lock.Enter();

            try
            {
                Instance<IFileConverter>.Value.Execute(
                    parameter.SrcFilePath, parameter.ExportFilePath, parameter.ImageFileFormat);
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
                Instance<IFileExporter>.Value.Lock.Exit();
            }
        }
    }
}
