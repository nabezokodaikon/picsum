using PicSum.Core.Task.AsyncTaskV2;

namespace PicSum.Task.Paramter
{
    public sealed class ExportFileParameter
        : ITaskParameter
    {
        public string SrcFilePath { get; set; }
        public string ExportFilePath { get; set; }
    }
}
