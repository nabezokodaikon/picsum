using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class FileConverter
        : IFileConverter
    {
        private bool disposed = false;
        public Lock Lock { get; private set; } = new Lock();

        public FileConverter()
        {

        }

        ~FileConverter()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {

            }

            this.disposed = true;
        }

        public void Execute(
            string srcFilePath,
            string exportFilePath,
            FileUtil.ImageFileFormat imageFileFormat)
        {
            ArgumentException.ThrowIfNullOrEmpty(srcFilePath, nameof(srcFilePath));
            ArgumentException.ThrowIfNullOrEmpty(exportFilePath, nameof(exportFilePath));

            ImageUtil.ConvertImageFile(
                srcFilePath, exportFilePath, imageFileFormat);
        }
    }
}
