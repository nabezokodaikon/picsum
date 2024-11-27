namespace SWF.Core.FileAccessor
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
