namespace SWF.Core.FileAccessor
{
    public interface IFileExporter
        : IDisposable
    {
        public Lock Lock { get; }
        public void Execute(string srcFilePath, string exportFilePath);
    }
}
