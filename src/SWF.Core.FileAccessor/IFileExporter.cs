namespace SWF.Core.FileAccessor
{
    public interface IFileExporter
        : IDisposable
    {
        public SemaphoreSlim Lock { get; }
        public void Execute(string srcFilePath, string exportFilePath);
    }
}
