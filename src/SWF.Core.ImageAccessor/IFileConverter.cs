using SWF.Core.FileAccessor;

namespace SWF.Core.ImageAccessor
{
    public interface IFileConverter
        : IDisposable
    {
        public Lock Lock { get; }
        public void Execute(
            string srcFilePath,
            string exportFilePath,
            FileUtil.ImageFileFormat imageFileFormat);
    }
}
